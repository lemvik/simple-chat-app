using System.ComponentModel.DataAnnotations;

namespace LemVic.Services.Chat.Protocol
{
    public class AuthParams
    {
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
