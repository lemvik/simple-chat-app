using System.Threading.Tasks;
using LemVic.Services.Chat.DataAccess.Models;

namespace LemVic.Services.Chat.Services
{
    public interface IAuthService
    {
        Task<(ChatUser User, string Token)> Authenticate(string userName, string password);

        Task<(ChatUser User, string Token)> Create(string userName, string password, string alias);
    }
}
