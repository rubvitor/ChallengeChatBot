using MediatR;
using System.Net.WebSockets;

namespace Challenge.ChatBot.Domain.Core.Commands
{
    public class ChatBotSessionCommand : IRequest<bool>
    {
        public string UserName { get; set; }
        public WebSocket WebSocket { get; set; }
    }
}
