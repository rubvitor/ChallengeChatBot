using Microsoft.Extensions.Options;

namespace Challenge.ChatBot.Util
{
    public class Constants
    {
        private readonly IOptions<ConfigurationRabbit> _config;
        public Constants(IOptions<ConfigurationRabbit> config)
        {
            _config = config;
        }

        public ConfigurationRabbit GetRabbitConfig()
        {
            return _config.Value;
        }
    }

    public class ConfigurationRabbit
    {
        public string Host { get; set; }
        public string Consumer { get; set; }
        public string Producer { get; set; }
    }
}