using System;
using System.Threading;
using System.Threading.Tasks;
using LemVic.Services.Chat.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LemVic.Services.Chat.Services
{
    public class RedisPresenceCleanupService : BackgroundService
    {
        private readonly IDatabaseAsync             RedisAsync;
        private readonly IOptions<PresenceSettings> Settings;

        public RedisPresenceCleanupService(IDatabaseAsync redisAsync, IOptions<PresenceSettings> settings)
        {
            RedisAsync = redisAsync;
            Settings   = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var timestamp = DateTime.UtcNow.ToFileTimeUtc();
                await RedisAsync.SortedSetRemoveRangeByScoreAsync(Settings.Value.PresenceKey, 0, timestamp);
                await Task.Delay(Settings.Value.PresenceUpdateInterval, stoppingToken);
            }
        }
    }
}
