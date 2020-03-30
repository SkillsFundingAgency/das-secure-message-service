using Microsoft.Extensions.Caching.Distributed;
using SFA.DAS.SecureMessageService.Core.IRepositories;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.SecureMessageService.Infrastructure.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDasDistributedCache _cache;

        public CacheRepository(IDasDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SaveAsync(string key, string message, int ttl)
        {
            // Save message to cache using given ttl
            await _cache.SetStringAsync(key, message, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(ttl)
            });
        }

        public async Task<string> RetrieveAsync(string key)
        {
            // Retrieve cached item
            var message = await _cache.GetStringAsync(key);

            // Remove cached item
            await _cache.RemoveAsync(key);

            return message;
        }

        public async Task<bool> TestAsync(string key)
        {
            // Retrieve but do not remove message
            var message = await _cache.GetStringAsync(key);

            return !string.IsNullOrEmpty(message);
        }
    }
}
