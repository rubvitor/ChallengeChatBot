using System.ComponentModel.DataAnnotations;

namespace Challenge.ChatBot.Domain.Core.Entities
{
    public class UserModel
    {
        [Key]
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Password  { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
