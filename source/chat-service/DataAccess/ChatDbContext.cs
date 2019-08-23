using LemVic.Services.Chat.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace LemVic.Services.Chat.DataAccess
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        public DbSet<ChatUser> Users { get; set; }
        public DbSet<UserAuth> Auths { get; set; }
    }
}
