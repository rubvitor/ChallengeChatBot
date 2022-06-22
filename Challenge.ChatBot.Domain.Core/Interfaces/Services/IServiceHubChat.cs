using Challenge.ChatBot.Domain.Core.Entities;
using System.Net.WebSockets;

namespace Challenge.ChatBot.Domain.Core.Interfaces.Services
{
    public interface IServiceHubChat
    {
        Task Add(string userName, string receiver, WebSocket ws);
        Task Remove(string userName);
        Task SendMessage(MessageModel message, WebSocketMessageType type, bool finish);
    }
}
