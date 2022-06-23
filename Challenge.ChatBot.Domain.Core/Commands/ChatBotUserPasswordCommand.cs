using Challenge.ChatBot.Domain.Core.Entities;
using MediatR;

namespace Challenge.ChatBot.Domain.Core.Commands
{
    public class ChatBotUserPasswordCommand : IRequest<UserModel>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
