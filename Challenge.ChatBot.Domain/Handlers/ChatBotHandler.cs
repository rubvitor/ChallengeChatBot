using Challenge.ChatBot.Domain.Core.Commands;
using Challenge.ChatBot.Domain.Core.Entities;
using Challenge.ChatBot.Domain.Core.Interfaces.Repositories;
using Challenge.ChatBot.Domain.Core.Interfaces.Services;
using Challenge.ChatBot.Domain.Core.Models;
using Challenge.ChatBot.Util;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Challenge.ChatBot.Domain.Handlers
{
    public class ChatBotHandler : IRequestHandler<ChatBotSendCommand, bool>,
                                  IRequestHandler<ChatBotGetCommand, List<MessageModel>>,
                                  IRequestHandler<ChatBotUserCommand, bool>,
                                  IRequestHandler<ChatBotRemoveCommand, bool>,
                                  IRequestHandler<ChatBotSessionCommand, bool>,
                                  IRequestHandler<ChatBotUserPasswordCommand, UserModel>
    {

        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IServiceHubChat _serviceHubChat;
        private readonly ILogger<ChatBotHandler> _logger;
        private readonly IErrorHandler _errorHandler;
        public ChatBotHandler(IUserRepository userRepository,
                              IMessageRepository messageRepository,
                              IServiceHubChat serviceHubChat,
                              IErrorHandler errorHandler,
                              ILogger<ChatBotHandler> logger)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _serviceHubChat = serviceHubChat;
            _logger = logger;
            _errorHandler = errorHandler;
        }

        public async Task<bool> Handle(ChatBotSendCommand request, CancellationToken ct)
        {
            return await _messageRepository.AddMessage(request.UserName, request.Message, request.Receiver);
        }

        public async Task<List<MessageModel>> Handle(ChatBotGetCommand request, CancellationToken ct)
        {
            return await _messageRepository.List(request.UserName);
        }

        public async Task<UserModel> Handle(ChatBotUserPasswordCommand request, CancellationToken ct)
        {
            return await _userRepository.VerifyUserAccess(request.UserName, request.Password);
        }

        public async Task<bool> Handle(ChatBotRemoveCommand request, CancellationToken cancellationToken)
        {
            return await _messageRepository.RemoveAll(request.UserName);
        }

        public async Task<bool> Handle(ChatBotSessionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var nameReceiver = new Bogus.DataSets.Name().FirstName();

                await _serviceHubChat.Add(request.UserName, nameReceiver, request.WebSocket);

                var result = await ReaderSocket(request, nameReceiver);

                await request.WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                await _serviceHubChat.Remove(request.UserName);

                _logger.Log(LogLevel.Information, $"Connection closed client {request.UserName} at {DateTime.Now}");

                return true;
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex).Wait();
                return false;
            }
        }

        private async Task<WebSocketReceiveResult> ReaderSocket(ChatBotSessionCommand request, string nameReceiver)
        {
            var buffer = new ArraySegment<byte>(new byte[1024 * 4]);
            var result = await request.WebSocket.ReceiveAsync(buffer, CancellationToken.None);

            _logger.Log(LogLevel.Information, $"Message received from user {request.UserName} at {DateTime.Now}");

            while (!result.CloseStatus.HasValue)
            {
                try
                {
                    var encodedBuffer = buffer.ArraySegmentByteToString();
                    buffer = new ArraySegment<byte>(new byte[1024 * 4]);

                    var messageViewModel = JsonConvert.DeserializeObject<MessageViewModel>(encodedBuffer);

                    var messageModel = new MessageModel(Guid.NewGuid(), messageViewModel.Message, messageViewModel.Username, nameReceiver, DateTime.Now);
                    await _messageRepository.AddMessage(messageModel);

                    await _serviceHubChat.SendMessage(messageModel, result.MessageType, result.EndOfMessage);

                    _logger.Log(LogLevel.Information, $"Message sent to {messageModel.UserName} at {DateTime.Now}");

                    result = await request.WebSocket.ReceiveAsync(buffer, CancellationToken.None);

                    _logger.Log(LogLevel.Information, $"Message received from {messageModel.UserName}");
                }
                catch (Exception ex)
                {
                    result = await request.WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    _errorHandler.Handle(ex).Wait();
                }
            }

            return result;
        }

        public async Task<bool> Handle(ChatBotUserCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.VerifyUserExist(request.UserName);
        }
    }
}
