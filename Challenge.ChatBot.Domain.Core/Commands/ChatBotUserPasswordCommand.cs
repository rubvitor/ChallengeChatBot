using MediatR;

namespace Challenge.ChatBot.Domain.Core.Commands
{
    public class ChatBotUserPasswordCommand : IRequest<bool>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
