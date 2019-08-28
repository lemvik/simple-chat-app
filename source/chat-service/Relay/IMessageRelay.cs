using System.Threading.Tasks;

namespace LemVic.Services.Chat.Relay
{
    public interface IMessageRelay
    {
        Task RelayUserMessage(string user, string message);
    }
}
