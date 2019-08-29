using LemVic.Services.Chat.DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LemVic.Services.Chat.DataAccess
{
    public class ChatDbContext : IdentityDbContext<ChatUser>
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }
    }
}
