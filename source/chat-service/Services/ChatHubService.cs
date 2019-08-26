using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using LemVic.Services.Chat.Hubs;
using LemVic.Services.Chat.Relay;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LemVic.Services.Chat.Services
{
    public class ChatHubService : BackgroundService
    {
        private readonly IHubContext<ChatHub>     ChatHubContext;
        private readonly IMessageRelay            MessageRelay;
        private readonly IChatUserCache           UserCache;
        private readonly IOptions<ChatHubOptions> Options;

        public ChatHubService(IHubContext<ChatHub>     chatHubContext,
                              IMessageRelay            messageRelay,
                              IChatUserCache           userCache,
                              IOptions<ChatHubOptions> options)
        {
            ChatHubContext = chatHubContext;
            MessageRelay   = messageRelay;
            Options        = options;
            UserCache      = userCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            MessageRelay.OnUserConnected(UserConnected);
            MessageRelay.OnUserDisconnected(UserDisconnected);
            MessageRelay.OnUserPostedMessage(UserPostedMessage);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Options.Value.UserReapInterval, stoppingToken);
            }
        }

        private async Task UserConnected(string userName)
        {
            UserCache.AddUser(userName, Options.Value.UserReapInterval);
            await ChatHubContext.Clients.All.SendAsync("UserConnected", UserCache.ExistingUsers);
        }

        private async Task UserDisconnected(string userName)
        {
            UserCache.RemoveUser(userName);
            await ChatHubContext.Clients.All.SendAsync("UserDisconnected", userName);
        }

        private async Task UserPostedMessage(string userName, string message)
        {
            UserCache.RefreshUser(userName, Options.Value.UserReapInterval);
            await ChatHubContext.Clients.All.SendAsync("ReceiveMessage", userName, message);
        }
    }
}
