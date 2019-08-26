using System;
using System.Threading.Tasks;

namespace LemVic.Services.Chat.Relay
{
    public interface IMessageRelay
    {
        Task RelayUserConnected(string user);

        Task RelayUserDisconnected(string user);

        Task RelayUserMessage(string user, string message);

        Task HubStatus(string[] users);

        void OnUserConnected(Func<string, Task> onUserConnected);

        void OnUserDisconnected(Func<string, Task> onUserDisconnected);

        void OnUserPostedMessage(Func<string, string, Task> onUserPostedMessage);

        void OnHubStatus(Func<string[], Task> onHubStatus);
    }
}
