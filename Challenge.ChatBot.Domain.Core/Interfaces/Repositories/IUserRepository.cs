namespace Challenge.ChatBot.Domain.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<bool> VerifyUserAccess(string username, string password);
        Task<bool> VerifyUserExist(string username);
    }
}
