using System.ComponentModel.DataAnnotations;

namespace LemVic.Services.Chat.Protocol
{
    public class CreateParams
    {
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Alias { get; set; }
    }
}
