using Challenge.ChatBot.Domain.Core.Entities;

namespace Challenge.ChatBot.Domain.Core.Interfaces.Repositories
{
    public interface IMessageRepository
    {
        Task<List<MessageModel>> List(string userName);
        Task<bool> AddMessage(string userName, string message, string receiver = null);
        Task<bool> AddMessage(MessageModel messageModel);
        Task<bool> RemoveAll(string userName);
    }
}
