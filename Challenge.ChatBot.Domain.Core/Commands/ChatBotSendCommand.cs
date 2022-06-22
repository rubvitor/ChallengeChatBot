using MediatR;

namespace Challenge.ChatBot.Domain.Core.Commands
{
    public class ChatBotSendCommand : IRequest<bool>
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public string Receiver { get; set; }
    }
}
