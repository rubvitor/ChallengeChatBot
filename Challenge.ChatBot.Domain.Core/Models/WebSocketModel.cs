using Challenge.ChatBot.Domain.Core.Interfaces.Services;
using System.Net.WebSockets;

namespace Challenge.ChatBot.Domain.Core.Models
{
    public class WebSocketModel : IWebSocketModel
    {
        public WebSocketModel()
        {
            List = new List<WebSocketModel>();
        }
        public string Receiver { get; set; }
        public string UserName { get; set; }
        public WebSocket WebSocket { get; set; }

        public List<WebSocketModel> List { get; set; }
    }
}
