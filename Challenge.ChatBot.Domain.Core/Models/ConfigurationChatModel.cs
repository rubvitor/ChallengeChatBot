using Challenge.ChatBot.Domain.Core.Entities;

namespace Challenge.ChatBot.Domain.Core.Models
{
    public class ConfigurationChatModel
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Consumer { get; set; }
        public string Producer { get; set; }
        public string StockUrl { get; set; }
        public List<UserModel> UserModels { get; set; }
    }
}
