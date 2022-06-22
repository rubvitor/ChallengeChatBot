using Challenge.ChatBot.Domain.Core.Interfaces.RabbitMQ;
using Challenge.ChatBot.Domain.Core.Models;
using Challenge.ChatBot.IOC;
using Challenge.ChatBot.Repository.Context;
using Microsoft.Extensions.Options;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDomains();

builder.Services.AddCors();

builder.Services.Configure<ConfigurationChatModel>(builder.Configuration.GetSection("Configuration"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(opt =>
{
    opt.AllowAnyHeader();
    opt.AllowAnyMethod();
    opt.AllowAnyOrigin();
});

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();

var connectionRabbit = app.Services.GetRequiredService<IRabbitBotConnection>();
await connectionRabbit.Connection();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<ChatBotContext>();
    var options = serviceScope.ServiceProvider.GetRequiredService<IOptions<ConfigurationChatModel>>();

    await InitializeDatabase.Initialize(context, options);
}

app.Run();
