using System.Threading;
using System.Threading.Tasks;
using LemVic.Services.Chat.Hubs;
using LemVic.Services.Chat.Relay;
using LemVic.Services.Chat.Utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace LemVic.Services.Chat.Services
{
    public class ChatMessagesBroadcaster : BackgroundService
    {
        private readonly IHubContext<ChatHub> ChatHubContext;
        private readonly IMessageRelay        MessageRelay;

        public ChatMessagesBroadcaster(IHubContext<ChatHub> chatHubContext, IMessageRelay messageRelay)
        {
            ChatHubContext = chatHubContext;
            MessageRelay   = messageRelay;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            MessageRelay.OnUserConnected(UserConnected);
            MessageRelay.OnUserDisconnected(UserDisconnected);
            MessageRelay.OnUserPostedMessage(UserPostedMessage);
            return stoppingToken.AwaitCancellation();
        }

        private async Task UserConnected(string userName)
        {
            await ChatHubContext.Clients.All.SendAsync("UserConnected", userName);
        }

        private async Task UserDisconnected(string userName)
        {
            await ChatHubContext.Clients.All.SendAsync("UserDisconnected", userName);
        }

        private async Task UserPostedMessage(string userName, string message)
        {
            await ChatHubContext.Clients.All.SendAsync("ReceiveMessage", userName, message);
        }
    }
}
