using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LemVic.Services.Chat.DataAccess.Models
{
    public class UserAuth : IdentityUser<string>
    {
        [Key]
        [Required]
        public string Login { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public ChatUser User { get; set; }
    }
}
