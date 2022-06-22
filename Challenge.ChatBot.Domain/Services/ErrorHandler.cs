using Challenge.ChatBot.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Challenge.ChatBot.Domain.Services
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly ILogger<ErrorHandler> _logger;
        public ErrorHandler(ILogger<ErrorHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}
