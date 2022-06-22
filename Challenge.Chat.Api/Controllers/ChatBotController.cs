using Challenge.ChatBot.Domain.Core.Commands;
using Challenge.ChatBot.Domain.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Challenge.Chat.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatBotController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatBotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/messages/{username}")]
        public List<MessageModel> GetMessages(string username)
        {
            if (!_mediator.Send(new ChatBotUserCommand { UserName = username }).Result)
                return null;

            return _mediator.Send(new ChatBotGetCommand { UserName = username }).Result;
        }

        [HttpGet("/websocket/{username}")]
        public void GetWebSocket(string username)
        {
            if (!_mediator.Send(new ChatBotUserCommand { UserName = username }).Result)
                return;

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = HttpContext.WebSockets.AcceptWebSocketAsync().Result;
                while (webSocket.State != WebSocketState.Open)
                    Thread.Sleep(1);

                _mediator.Send(new ChatBotSessionCommand { UserName = username, WebSocket = webSocket }).Wait();
            }
        }
    }
}