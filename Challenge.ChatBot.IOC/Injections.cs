using Challenge.ChatBot.Domain.Core.Interfaces.RabbitMQ;
using Challenge.ChatBot.Domain.Core.Interfaces.Repositories;
using Challenge.ChatBot.Domain.Core.Interfaces.Services;
using Challenge.ChatBot.Domain.Core.Models;
using Challenge.ChatBot.Domain.Handlers;
using Challenge.ChatBot.Domain.Services;
using Challenge.ChatBot.Repository.Context;
using Challenge.ChatBot.Repository.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.ChatBot.IOC
{
    public static class Injections
    {
        public static void AddDomains(this IServiceCollection services)
        {
            services.AddDbContext<ChatBotContext>(opt => opt.UseInMemoryDatabase("ChatBoxDatabase"));

            services.AddSingleton<IServiceHubChat, ServiceHubChat>();
            services.AddSingleton<IServiceGetStock, ServiceGetStock>();
            services.AddSingleton<IRabbitBotConnection, RabbitBotConnection>();
            services.AddSingleton<IWebSocketModel, WebSocketModel>();
            services.AddSingleton<HttpClient>(new HttpClient());
            services.AddSingleton<IErrorHandler, ErrorHandler>();

            services.AddMediatR(typeof(ChatBotHandler));

            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}