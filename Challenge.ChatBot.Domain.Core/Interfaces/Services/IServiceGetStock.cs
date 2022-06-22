using Challenge.ChatBot.Domain.Core.Models;

namespace Challenge.ChatBot.Domain.Core.Interfaces.Services
{
    public interface IServiceGetStock
    {
        Task<MessageChannelModel> GetChannel(string message);
    }
}
