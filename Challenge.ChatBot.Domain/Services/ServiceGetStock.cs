using Challenge.ChatBot.Domain.Core.Interfaces.Services;
using Challenge.ChatBot.Domain.Core.Models;
using Challenge.ChatBot.Util;
using Microsoft.Extensions.Options;

namespace Challenge.ChatBot.Domain.Services
{
    public class ServiceGetStock : IServiceGetStock
    {
        private readonly HttpClient _httpClient;
        private readonly ConfigurationChatModel _config;
        public ServiceGetStock(HttpClient httpClient,
                               IOptions<ConfigurationChatModel> config)
        {
            _httpClient = httpClient;
            _config = config.Value;
        }

        public async Task<MessageChannelModel> GetChannel(string message)
        {
            var url = string.Format(_config.StockUrl, message);

            var response = await _httpClient.GetAsync(url);

            var stringResponse = await response.Content.ReadAsStringAsync();

            var messageChannel = stringResponse.Transform<MessageChannelModel>();

            return messageChannel?.FirstOrDefault();
        }
    }
}
