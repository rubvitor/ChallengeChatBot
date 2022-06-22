namespace Challenge.ChatBot.Domain.Core.Interfaces.Services
{
    public interface IErrorHandler
    {
        Task Handle(Exception ex);
    }
}
