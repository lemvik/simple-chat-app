using System.Threading.Tasks;

namespace LemVic.Services.Chat.Relay
{
    public class DevNullMessageRelay : IMessageRelay
    {
        public Task RelayUserMessage(string user, string message)
        {
            return Task.CompletedTask;
        }
    }
}
