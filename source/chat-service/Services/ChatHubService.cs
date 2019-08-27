using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IHubContext<ChatHub>      ChatHubContext;
        private readonly IMessageRelay             MessageRelay;
        private readonly IChatPresenceService      PresenceService;
        private readonly IDictionary<string, bool> LocalPresence;
        private readonly IOptions<ChatHubOptions>  Options;

        public ChatHubService(IHubContext<ChatHub>     chatHubContext,
                              IMessageRelay            messageRelay,
                              IChatPresenceService     presenceService,
                              IOptions<ChatHubOptions> options)
        {
            ChatHubContext  = chatHubContext;
            MessageRelay    = messageRelay;
            PresenceService = presenceService;
            LocalPresence   = new ConcurrentDictionary<string, bool>();
            Options         = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            MessageRelay.OnUserConnected(UserConnected);
            MessageRelay.OnUserDisconnected(UserDisconnected);
            MessageRelay.OnUserPostedMessage(UserPostedMessage);
            MessageRelay.OnHubStatus(UpdateHubStatus);

            while (!stoppingToken.IsCancellationRequested)
            {
                await SendHubStatus(stoppingToken);
                await UpdateClientPresence(stoppingToken);
                await Task.Delay(Options.Value.PresenceUpdateInterval, stoppingToken);
            }
        }

        private async Task UserConnected(string userName)
        {
            LocalPresence[userName] = true;
            await PresenceService.AddUser(userName, Options.Value.PlayerPresenceTimeout);
            await UpdateClientPresence();
        }

        private async Task UserDisconnected(string userName)
        {
            LocalPresence.Remove(userName);
            await PresenceService.RemoveUser(userName);
            await UpdateClientPresence();
        }

        private async Task UserPostedMessage(string userName, string message)
        {
            await PresenceService.RefreshUser(userName, Options.Value.PlayerPresenceTimeout);
            await ChatHubContext.Clients.All.SendAsync("ReceiveMessage", userName, message);
        }

        private async Task UpdateClientPresence(CancellationToken token = default)
        {
            var presentUsers = await PresenceService.ListExistingUsers();
            await ChatHubContext.Clients.All.SendAsync("Presence", presentUsers, token);
        }

        private async Task UpdateHubStatus(string[] users)
        {
            foreach (var user in users)
            {
                await PresenceService.RefreshUser(user, Options.Value.PlayerPresenceTimeout);
            }
        }

        private Task SendHubStatus(CancellationToken token = default)
        {
            return MessageRelay.HubStatus(LocalPresence.Keys.ToArray());
        }
    }
}
