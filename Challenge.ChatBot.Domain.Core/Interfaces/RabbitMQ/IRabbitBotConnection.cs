namespace Challenge.ChatBot.Domain.Core.Interfaces.RabbitMQ
{
    public interface IRabbitBotConnection
    {
        Task Connection();
    }
}
