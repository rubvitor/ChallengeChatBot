using Challenge.ChatBot.Domain.Core.Entities;
using Challenge.ChatBot.Domain.Core.Interfaces;
using Challenge.ChatBot.Domain.Core.Interfaces.Repositories;
using Challenge.ChatBot.Domain.Core.Models;
using Challenge.ChatBot.Repository.Context;
using Challenge.ChatBot.Util;
using Microsoft.Extensions.Options;

namespace Challenge.ChatBot.IOC
{
    public static class InitializeDatabase
    {
        public static async Task Initialize(ChatBotContext chatBotContext,
                                            IOptions<ConfigurationChatModel> options)
        {
            foreach(var userModel in options.Value.UserModels)
            {
                userModel.Password = userModel.Password.EncriptPassword();
                await chatBotContext.Users.AddAsync(userModel);
            }

            await chatBotContext.SaveChangesAsync();
        }
    }
}
