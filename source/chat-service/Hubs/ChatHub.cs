using System;
using System.Threading.Tasks;
using LemVic.Services.Chat.Relay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LemVic.Services.Chat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageRelay MessageRelay;

        public ChatHub(IMessageRelay messageRelay)
        {
            MessageRelay = messageRelay;
        }

        public async Task SendMessage(string user, string message)
        {
            await MessageRelay.RelayUserMessage(user, message);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await MessageRelay.RelayUserConnected(Context.User.Identity.Name);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            await MessageRelay.RelayUserDisconnected(Context.User.Identity.Name);
        }
    }
}
