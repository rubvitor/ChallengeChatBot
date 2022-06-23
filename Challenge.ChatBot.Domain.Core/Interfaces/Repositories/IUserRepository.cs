using Challenge.ChatBot.Domain.Core.Entities;

namespace Challenge.ChatBot.Domain.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserModel> VerifyUserAccess(string username, string password);
        Task<bool> VerifyUserExist(string username);
    }
}
