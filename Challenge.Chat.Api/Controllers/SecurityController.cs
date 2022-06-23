using Challenge.ChatBot.Domain.Core.Commands;
using Challenge.ChatBot.Domain.Core.Entities;
using Challenge.ChatBot.Domain.Handlers;
using Challenge.ChatBot.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Chat.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class SecurityController : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public ActionResult<dynamic> Authenticate([FromServices] IMediator mediator, [FromBody] UserModel model)
        {
            var user = mediator.Send(new ChatBotUserPasswordCommand { UserName = model.UserName, Password = model.Password }).Result;

            if (user is null)
                return NotFound(new { message = "User or Password Invalid." });

            var token = TokenService.GenerateToken(user);

            user.Password = string.Empty;

            return new
            {
                user = user,
                token = token
            };
        }
    }
}
