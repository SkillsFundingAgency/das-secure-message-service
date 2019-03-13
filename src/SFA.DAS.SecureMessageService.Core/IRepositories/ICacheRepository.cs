using System.Threading.Tasks;

namespace SFA.DAS.SecureMessageService.Core.IRepositories
{
    public interface ICacheRepository
    {
        Task SaveAsync(string key, string message, int ttl);
        Task<string> RetrieveAsync(string key);
        Task<bool> TestAsync(string token);
    }
}