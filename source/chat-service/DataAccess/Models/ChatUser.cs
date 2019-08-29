using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LemVic.Services.Chat.DataAccess.Models
{
    public class ChatUser : IdentityUser
    {
        [Required]
        public string Alias { get; set; }
    }
}
