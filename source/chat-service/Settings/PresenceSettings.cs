using System;

namespace LemVic.Services.Chat.Settings
{
    public class PresenceSettings
    {
        public string   PresenceKey            { get; set; } = "chat-app-presence";
        public TimeSpan PresenceUpdateInterval { get; set; } = TimeSpan.FromSeconds(15);
    }
}
