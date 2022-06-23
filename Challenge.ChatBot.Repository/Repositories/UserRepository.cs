using Challenge.ChatBot.Domain.Core.Entities;
using Challenge.ChatBot.Domain.Core.Interfaces.Repositories;
using Challenge.ChatBot.Repository.Context;
using Challenge.ChatBot.Util;
using Microsoft.EntityFrameworkCore;

namespace Challenge.ChatBot.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbSet<UserModel> Users;
        public UserRepository(ChatBotContext chatBotContext)
        {
            Users = chatBotContext.Users;
        }

        public async Task<UserModel> VerifyUserAccess(string username, string password)
        {
            return await Users?.Where(x => x.UserName.Equals(username) && x.Password.ComparePassword(password))?.FirstAsync();
        }

        public async Task<bool> VerifyUserExist(string username)
        {
            return await Users.AnyAsync(x => x.UserName.Equals(username));
        }
    }
}
