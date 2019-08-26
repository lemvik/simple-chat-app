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
        private readonly IChatPresenceService     PresenceService;
        private readonly IOptions<ChatHubOptions> Options;

        public ChatHubService(IHubContext<ChatHub>     chatHubContext,
                              IMessageRelay            messageRelay,
                              IChatPresenceService     presenceService,
                              IOptions<ChatHubOptions> options)
        {
            ChatHubContext  = chatHubContext;
            MessageRelay    = messageRelay;
            Options         = options;
            PresenceService = presenceService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            MessageRelay.OnUserConnected(UserConnected);
            MessageRelay.OnUserDisconnected(UserDisconnected);
            MessageRelay.OnUserPostedMessage(UserPostedMessage);

            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateClientPresence(stoppingToken);
                await Task.Delay(Options.Value.UserReapInterval, stoppingToken);
            }
        }

        private async Task UserConnected(string userName)
        {
            await PresenceService.AddUser(userName, Options.Value.UserReapInterval);
            await UpdateClientPresence();
        }

        private async Task UserDisconnected(string userName)
        {
            await PresenceService.RemoveUser(userName);
            await UpdateClientPresence();
        }

        private async Task UserPostedMessage(string userName, string message)
        {
            await PresenceService.RefreshUser(userName, Options.Value.UserReapInterval);
            await ChatHubContext.Clients.All.SendAsync("ReceiveMessage", userName, message);
        }

        private async Task UpdateClientPresence(CancellationToken token = default)
        {
            var presentUsers = await PresenceService.ListExistingUsers();
            await ChatHubContext.Clients.All.SendAsync("Presence", presentUsers, token);
        }
    }
}
