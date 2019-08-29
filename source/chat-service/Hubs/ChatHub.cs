using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LemVic.Services.Chat.Relay;
using LemVic.Services.Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LemVic.Services.Chat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatPresenceService PresenceService;
        private readonly IMessageRelay        MessageRelay;

        public ChatHub(IChatPresenceService presenceService, IMessageRelay messageRelay)
        {
            PresenceService = presenceService;
            MessageRelay    = messageRelay;
        }

        public async Task SendMessage(string message)
        {
            var userAlias = Context.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            await Clients.All.SendAsync("ReceiveMessage", userAlias, message);
            await PresenceService.RefreshUser(userAlias, TimeSpan.FromMinutes(1));
            await MessageRelay.RelayUserMessage(userAlias, message);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var userAlias = Context.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            await PresenceService.AddUser(userAlias, TimeSpan.FromMinutes(1));
            var users = await PresenceService.ListExistingUsers();
            await Clients.All.SendAsync("Presence", users);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            var userAlias = Context.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            await PresenceService.RemoveUser(userAlias);
            var users = await PresenceService.ListExistingUsers();
            await Clients.All.SendAsync("Presence", users);
        }
    }
}
