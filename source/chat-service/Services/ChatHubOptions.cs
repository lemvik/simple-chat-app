using System;

namespace LemVic.Services.Chat.Services
{
    public class ChatHubOptions
    {
        public TimeSpan PresenceUpdateInterval { get; set; } = TimeSpan.FromSeconds(15);
        public TimeSpan PlayerPresenceTimeout  { get; set; } = TimeSpan.FromSeconds(30);
    }
}
