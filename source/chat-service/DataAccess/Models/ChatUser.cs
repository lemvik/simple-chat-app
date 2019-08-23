using System.ComponentModel.DataAnnotations;

namespace LemVic.Services.Chat.DataAccess.Models
{
    public class ChatUser
    {
        public long Id { get; set; }

        [Required]
        public string Alias { get; set; }
    }
}
