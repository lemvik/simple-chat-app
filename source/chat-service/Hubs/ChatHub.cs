using System;
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

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            await PresenceService.RefreshUser(Context.UserIdentifier, TimeSpan.FromMinutes(1));
            await MessageRelay.RelayUserMessage(user, message);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await PresenceService.AddUser(Context.UserIdentifier, TimeSpan.FromMinutes(1));
            var users = await PresenceService.ListExistingUsers();
            await Clients.All.SendAsync("Presence", users);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            await PresenceService.RemoveUser(Context.UserIdentifier);
            var users = await PresenceService.ListExistingUsers();
            await Clients.All.SendAsync("Presence", users);
        }
    }
}
