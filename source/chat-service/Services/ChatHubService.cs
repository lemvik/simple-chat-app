using System.Threading;
using System.Threading.Tasks;
using LemVic.Services.Chat.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LemVic.Services.Chat.Services
{
    public class ChatHubService : BackgroundService
    {
        private readonly IHubContext<ChatHub>     ChatHubContext;
        private readonly IChatPresenceService     PresenceService;
        private readonly IOptions<ChatHubOptions> Options;

        public ChatHubService(IHubContext<ChatHub>     chatHubContext,
                              IChatPresenceService     presenceService,
                              IOptions<ChatHubOptions> options)
        {
            ChatHubContext  = chatHubContext;
            PresenceService = presenceService;
            Options         = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateClientPresence(stoppingToken);
                await Task.Delay(Options.Value.PresenceUpdateInterval, stoppingToken);
            }
        }

        private async Task UpdateClientPresence(CancellationToken token = default)
        {
            var presentUsers = await PresenceService.ListExistingUsers();
            await ChatHubContext.Clients.All.SendAsync("Presence", presentUsers, token);
        }
    }
}
