using Challenge.ChatBot.Domain.Core.Entities;
using Challenge.ChatBot.Domain.Core.Interfaces.Repositories;
using Challenge.ChatBot.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Challenge.ChatBot.Repository.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatBotContext _chatBotContext;
        private readonly DbSet<MessageModel> messageModels;
        public MessageRepository(ChatBotContext chatBotContext)
        {
            _chatBotContext = chatBotContext;
            messageModels = _chatBotContext.Messages;
        }

        public async Task<List<MessageModel>> List(string userName)
        {
            var messagesGeral = await messageModels.ToListAsync();
            var messages = await messageModels.Where(x => x.UserName.Equals(userName)).OrderBy(x => x.DateMessage).Take(50).ToListAsync();
            return messages;
        }

        public async Task<bool> AddMessage(string userName, string message, string receiver = null, string actor = null)
        {
            await messageModels.AddAsync(new MessageModel
            {
                UserName = userName,
                Id = Guid.NewGuid(),
                Message = message,
                DateMessage = DateTime.Now,
                Receiver = receiver,
                Actor = actor
            });

            return await _chatBotContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddMessage(MessageModel messageModel)
        {
            await messageModels.AddAsync(messageModel);

            return await _chatBotContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveAll(string userName)
        {
            var messages = await messageModels.Where(m => m.UserName.Equals(userName)).ToListAsync();
            messageModels.RemoveRange(messages);

            return await _chatBotContext.SaveChangesAsync() > 0;
        }
    }
}
