using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace LemVic.Services.Chat.Services
{
    public class InMemoryChatPresenceService : IChatPresenceService
    {
        private readonly IMemoryCache              MemoryCache;
        private readonly IDictionary<string, bool> EntryKeys;

        public InMemoryChatPresenceService(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
            EntryKeys   = new ConcurrentDictionary<string, bool>();
        }

        public Task AddUser(string userName, TimeSpan expirationTime)
        {
            var newEntry = MemoryCache.CreateEntry(userName);
            newEntry.RegisterPostEvictionCallback(RemoveEntry);
            EntryKeys[userName] = true;
            return Task.CompletedTask;
        }

        public Task RefreshUser(string userName, TimeSpan expirationTime)
        {
            var newEntry = MemoryCache.CreateEntry(userName);
            newEntry.RegisterPostEvictionCallback(RemoveEntry);
            EntryKeys[userName] = true;
            return Task.CompletedTask;
        }

        public Task RemoveUser(string userName)
        {
            MemoryCache.Remove(userName);
            EntryKeys.Remove(userName);
            return Task.CompletedTask;
        }

        public Task<string[]> ListExistingUsers()
        {
            return Task.FromResult(EntryKeys.Keys.ToArray());
        }

        private void RemoveEntry(object entry, object ignored, EvictionReason reason, object ignoredState)
        {
            EntryKeys.Remove((string) entry);
        }
    }
}
