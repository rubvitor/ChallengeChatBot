using Challenge.ChatBot.Domain.Core.Entities;
using Challenge.ChatBot.Domain.Core.Interfaces.Repositories;
using Challenge.ChatBot.Domain.Core.Interfaces.Services;
using Challenge.ChatBot.Domain.Core.Models;
using Challenge.ChatBot.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.WebSockets;
using System.Text;
using System.Net;

namespace Challenge.ChatBot.Domain.Services
{
    public class ServiceHubChat : IServiceHubChat
    {
        private readonly IModel channelModel;
        private readonly ConfigurationChatModel rabbitConfigModel;
        private readonly IWebSocketModel _webSocketModel;
        private readonly IErrorHandler _errorHandler;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ServiceHubChat(IOptions<ConfigurationChatModel> config,
                              IErrorHandler errorHandler,
                              IWebSocketModel webSocketModel,
                              IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _errorHandler = errorHandler;
            rabbitConfigModel = config.Value;
            _webSocketModel = webSocketModel;
            _webSocketModel.List.Clear();

            string hostName = Dns.GetHostName();
            string ip = Dns.GetHostEntry(hostName)?.AddressList?.FirstOrDefault()?.ToString();

            var factory = new ConnectionFactory()
            {
                HostName = ip,
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
                        DateMessage = DateTime.Now,
                        Message = message.MessageReturn,
                        Receiver = message.Receiver,
                        Actor = message.Receiver,
                        Id = Guid.NewGuid(),
                        UserName = message.UserName
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
                var messageNew = new MessageModel
                {
                    Actor = message.Receiver,
                    Receiver = message.Receiver,
                    DateMessage = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Message = message.Message,
                    UserName = message.UserName
                };

                if (messageNew.Message.Contains("/stock="))
                {
                    messageNew.Message = message.Message.Replace("/stock=", string.Empty);
                    var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageNew));

                    channelModel.BasicPublish(
                        exchange: "",
                        routingKey: rabbitConfigModel.Producer,
                        body: messageBytes
                    );

                    return;
                }

                await FinishWebSockets(messageNew, type, finish, CancellationToken.None);
            }
            catch (Exception ex)
            {
                await _errorHandler.Handle(ex);
            }
        }

        private async Task FinishWebSockets(MessageModel message, WebSocketMessageType type, bool finish, CancellationToken ct)
        {
            _ = Task.Run(() =>
             {
                 using (var scope = _serviceScopeFactory.CreateScope())
                 {
                     var _messageRepository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                     _messageRepository.AddMessage(message).Wait();
                 }
             });

            foreach (var socket in _webSocketModel.List?.Where(w => w.UserName.Equals(message.UserName)))
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
