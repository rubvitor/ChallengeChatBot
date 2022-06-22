using Challenge.ChatBot.Domain.Core.Models;

namespace Challenge.ChatBot.Domain.Core.Interfaces.Services
{
    public interface IWebSocketModel
    {
        List<WebSocketModel> List { get; set; }
    }
}
