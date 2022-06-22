using Challenge.ChatBot.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Challenge.ChatBot.Repository.Context
{
    public class ChatBotContext : DbContext
    {
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<UserModel> Users { get; set; }

        public ChatBotContext(DbContextOptions<ChatBotContext> options) : base(options)
        {
        }
    }
}
