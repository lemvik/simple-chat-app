using System;

namespace LemVic.Services.Chat.Services
{
    public class ChatHubOptions
    {
        public TimeSpan UserReapInterval { get; set; } = TimeSpan.FromSeconds(15);
    }
}
