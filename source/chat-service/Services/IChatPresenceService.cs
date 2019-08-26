using System;
using System.Threading.Tasks;

namespace LemVic.Services.Chat.Services
{
    public interface IChatPresenceService
    {
        Task AddUser(string     userName, TimeSpan expirationTime);
        Task RefreshUser(string userName, TimeSpan expirationTime);
        Task RemoveUser(string  userName);

        Task<string[]> ListExistingUsers();
    }
}
