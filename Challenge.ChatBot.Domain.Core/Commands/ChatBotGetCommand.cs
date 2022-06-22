using Challenge.ChatBot.Domain.Core.Entities;
using MediatR;

namespace Challenge.ChatBot.Domain.Core.Commands
{
    public class ChatBotGetCommand : IRequest<List<MessageModel>>
    {
        public string UserName { get; set; }
    }
}
