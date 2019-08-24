using System.Threading.Tasks;

namespace LemVic.Services.Chat.Relay
{
    public interface IMessageRelay
    {
        Task RelayUserConnected(string user);

        Task RelayUserDisconnected(string user);

        Task RelayUserMessage(string user, string message);
    }
}
