using Challenge.ChatBot.Domain.Core.Models;
using Challenge.ChatBot.IOC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Challenge.ChatBot.Test
{
    public class Startup
    {
        public Startup()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Services.AddDomains();

            builder.Services.Configure<ConfigurationChatModel>(builder.Configuration.GetSection("Configuration"));

            Application = builder.Build();

            Task.Run(() => Application.Run());
        }

        public readonly WebApplication Application;
    }

    public class InjectionFixture : IDisposable
    {
        private readonly TestServer server;

        public InjectionFixture()
        {
            var startup = new Startup();
            server = new TestServer(startup.Application.Services);
        }

        public IServiceProvider ServiceProvider => server.Services;

        public void Dispose()
        {
            server.Dispose();
        }
    }
}