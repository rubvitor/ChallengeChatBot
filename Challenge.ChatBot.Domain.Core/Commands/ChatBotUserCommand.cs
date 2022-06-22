using MediatR;

namespace Challenge.ChatBot.Domain.Core.Commands
{
    public class ChatBotUserCommand : IRequest<bool>
    {
        public string UserName { get; set; }
    }
}
