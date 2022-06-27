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

    public static class Settings
    {
        public static string Secret = "fedaf7d8863b48e197b9287d492b708e";
    }

    public static class Security
    {
        public static string Key = "djaksdjaskdlj2735q739123";
    }

    public class DockerConfig
    {
        public DockerConfig(bool isDevelopment)
        {
            IsDevelopment = isDevelopment;
        }
        public bool IsDevelopment { get; set; }
    }
}