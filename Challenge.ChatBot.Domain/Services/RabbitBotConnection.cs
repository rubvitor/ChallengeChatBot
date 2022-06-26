using Challenge.ChatBot.Domain.Core.Entities;
using Challenge.ChatBot.Domain.Core.Interfaces.RabbitMQ;
using Challenge.ChatBot.Domain.Core.Interfaces.Services;
using Challenge.ChatBot.Domain.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Net;

namespace Challenge.ChatBot.Domain.Services
{
    public class RabbitBotConnection : IRabbitBotConnection
    {
        private readonly ConfigurationChatModel _config;
        private readonly IServiceGetStock _serviceHub;
        private readonly IErrorHandler _errorHandler;
        public RabbitBotConnection(IOptions<ConfigurationChatModel> config,
                                   IServiceGetStock serviceHub,
                                   IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
            _serviceHub = serviceHub;
            _config = config?.Value;
        }

        public async Task Connection()
        {
        string hostName = Dns.GetHostName();  

    Console.WriteLine(hostName);   

      

    // Get the IP from GetHostByName method of dns class. 

    string IP = Dns.GetHostByName(hostName).AddressList[0].ToString();   
            var fconnectionFctory = new ConnectionFactory()
            {
                HostName = IP,
                Port = _config.Port,
                UserName = _config.User,
                Password = _config.Password
            };

            var connection = fconnectionFctory.CreateConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: _config.Producer,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var eventingBasicConsumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(
                consumer: eventingBasicConsumer,
                queue: _config.Producer,
                autoAck: true
            );

            eventingBasicConsumer.Received += (@object, rec) =>
            {
                try
                {
                    Console.WriteLine($"Received Message: {rec.DeliveryTag}");

                    var properties = rec.BasicProperties;
                    var bodyArray = rec.Body.ToArray();
                    var basicProperties = channel.CreateBasicProperties();
                    basicProperties.CorrelationId = properties.CorrelationId;

                    var messageRecevied = Encoding.Default.GetString(bodyArray);
                    var messageDeserialized = JsonConvert.DeserializeObject<MessageModel>(messageRecevied);

                    var returnChannel = _serviceHub.GetChannel(messageDeserialized.Message).Result;
                    returnChannel.Receiver = messageDeserialized.Receiver;
                    returnChannel.UserName = messageDeserialized.UserName;

                    Console.WriteLine($"Message Processed from HUB: {returnChannel} at {DateTime.Now} User {messageDeserialized.UserName}");

                    var responseBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(returnChannel));

                    channel.BasicPublish(
                        exchange: string.Empty,
                        routingKey: _config.Consumer,
                        body: responseBytes
                    );
                }
                catch (Exception ex)
                {
                    _errorHandler.Handle(ex);
                }
            };

            Console.WriteLine("Waiting...");
        }
    }
}
