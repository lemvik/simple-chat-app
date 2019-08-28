using System;
using System.Threading.Tasks;
using LemVic.Services.Chat.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LemVic.Services.Chat.Services
{
    public class RedisChatPresenceService : IChatPresenceService
    {
        private readonly IDatabaseAsync             RedisAsync;
        private readonly IOptions<PresenceSettings> Settings;

        public RedisChatPresenceService(IDatabaseAsync redisAsync, IOptions<PresenceSettings> settings)
        {
            RedisAsync = redisAsync;
            Settings   = settings;
        }

        public async Task AddUser(string userName, TimeSpan expirationTime)
        {
            var timestamp = DateTime.UtcNow.Add(expirationTime).ToFileTimeUtc();
            await RedisAsync.SortedSetAddAsync(Settings.Value.PresenceKey, userName, timestamp);
        }

        public async Task RefreshUser(string userName, TimeSpan expirationTime)
        {
            var timestamp = DateTime.UtcNow.Add(expirationTime).ToFileTimeUtc();
            await RedisAsync.SortedSetAddAsync(Settings.Value.PresenceKey, userName, timestamp);
        }

        public async Task RemoveUser(string userName)
        {
            await RedisAsync.SortedSetRemoveAsync(Settings.Value.PresenceKey, userName);
        }

        public async Task<string[]> ListExistingUsers()
        {
            var timestamp = DateTime.UtcNow.ToFileTimeUtc();
            var elements  = await RedisAsync.SortedSetRangeByScoreAsync(Settings.Value.PresenceKey, timestamp);
            return elements.ToStringArray();
        }
    }
}
