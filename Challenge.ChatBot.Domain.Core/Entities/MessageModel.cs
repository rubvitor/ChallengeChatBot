using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challenge.ChatBot.Domain.Core.Entities
{
    public class MessageModel
    {
        public MessageModel() { }

        public MessageModel(Guid id, string message, string username, string receiver, DateTime dateMessage)
        {
            this.Id = id;
            this.Message = message;
            this.UserName = username;
            this.Receiver = receiver;
            this.DateMessage = dateMessage;
        }

        [Key]
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public string Receiver { get; set; }
        public DateTime DateMessage { get; set; }
        [ForeignKey("UserName")]
        public UserModel User { get; set; }
    }
}
