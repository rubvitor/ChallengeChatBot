using Challenge.ChatBot.Domain.Core.Entities;
using Challenge.ChatBot.Domain.Core.Interfaces.Services;
using Challenge.ChatBot.Domain.Core.Models;
using Challenge.ChatBot.Util;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.WebSockets;
using System.Text;

namespace Challenge.ChatBot.Domain.Services
{
    public class ServiceHubChat : IServiceHubChat
    {
        private readonly IModel channelModel;
        private readonly ConfigurationChatModel rabbitConfigModel;
        private readonly IWebSocketModel _webSocketModel;
        private readonly IErrorHandler _errorHandler;

        public ServiceHubChat(IOptions<ConfigurationChatModel> config,
                              IErrorHandler errorHandler,
                              IWebSocketModel webSocketModel)
        {
            _errorHandler = errorHandler;
            rabbitConfigModel = config.Value;
            _webSocketModel = webSocketModel;
            _webSocketModel.List.Clear();

            var factory = new ConnectionFactory()
            {
                HostName = rabbitConfigModel.Host,
                Port = rabbitConfigModel.Port,
                UserName = rabbitConfigModel.User,
                Password = rabbitConfigModel.Password
            };

            var connection = factory.CreateConnection();

            channelModel = connection.CreateModel();

            channelModel.QueueDeclare(
                queue: rabbitConfigModel.Consumer,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(channelModel);

            channelModel.BasicConsume(
                consumer: consumer,
                queue: rabbitConfigModel.Consumer,
                autoAck: true
            );

            consumer.Received += (objectModel, basicDeliver) =>
            {
                try
                {
                    var bodyArray = basicDeliver.Body.ToArray();
                    var response = Encoding.UTF8.GetString(bodyArray);

                    var message = JsonConvert.DeserializeObject<MessageChannelModel>(response);

                    var messageModel = new MessageModel
                    {
                        DateMessage = Convert.ToDateTime(message.Date),
                        Message = message.MessageReturn,
                        Receiver = message.Receiver
                    };

                    SendMessage(messageModel, WebSocketMessageType.Text, true);
                }
                catch (Exception ex)
                {
                    _errorHandler.Handle(ex).Wait();
                }
            };
        }

        public async Task SendMessage(MessageModel message, WebSocketMessageType type, bool finish)
        {
            try
            {
                if (message.Message.Contains("/stock="))
                {
                    message.Message = message.Message.Replace("/stock=", string.Empty);
                    var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                    channelModel.BasicPublish(
                        exchange: "",
                        routingKey: rabbitConfigModel.Producer,
                        body: messageBytes
                    );

                    return;
                }

                await FinishWebSockets(message, type, finish, CancellationToken.None);
            }
            catch (Exception ex)
            {
                await _errorHandler.Handle(ex);
            }
        }

        private async Task FinishWebSockets(MessageModel message, WebSocketMessageType type, bool finish, CancellationToken ct)
        {
            foreach (var socket in _webSocketModel.List)
                await socket.WebSocket.SendAsync(message.ToArraySegment(), type, finish, ct);
        }

        public async Task Add(string userName, string receiver, WebSocket ws)
        {
            try
            {
                _webSocketModel.List.Add(new WebSocketModel
                {
                    Receiver = receiver,
                    UserName = userName,
                    WebSocket = ws
                });
            }
            catch (Exception ex)
            {
                await _errorHandler.Handle(ex);
            }
        }

        public async Task Remove(string userName)
        {
            try
            {
                _webSocketModel.List.RemoveAll(w => w.UserName.Equals(userName));
            }
            catch (Exception ex)
            {
                await _errorHandler.Handle(ex);
            }
        }
    }
}
