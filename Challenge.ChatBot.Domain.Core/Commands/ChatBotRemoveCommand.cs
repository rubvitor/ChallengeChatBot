using MediatR;

namespace Challenge.ChatBot.Domain.Core.Commands
{
    public class ChatBotRemoveCommand : IRequest<bool>
    {
        public string UserName { get; set; }
    }
}
