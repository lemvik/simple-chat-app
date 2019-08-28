namespace LemVic.Services.Chat.Settings
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; }
        public string ChannelPrefix    { get; set; }
        public string PresenceKey      { get; set; }
    }
}
