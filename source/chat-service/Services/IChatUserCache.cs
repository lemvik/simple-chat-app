using System;

namespace LemVic.Services.Chat.Services
{
    public interface IChatUserCache
    {
        void AddUser(string     userName, TimeSpan expirationTime);
        void RefreshUser(string userName, TimeSpan expirationTime);
        void RemoveUser(string  userName);

        string[] ExistingUsers { get; }
    }
}
