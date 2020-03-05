using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace SFA.DAS.SecureMessageService.Core.IRepositories
{
    public interface IDasDistributedCache
    {
        Task SetStringAsync(string key, string message, DistributedCacheEntryOptions distributedCacheEntryOptions);

        Task<string> GetStringAsync(string key);

        Task RemoveAsync(string key);
    }
}
