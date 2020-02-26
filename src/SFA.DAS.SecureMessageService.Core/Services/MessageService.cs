using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Core.IRepositories;

namespace SFA.DAS.SecureMessageService.Core.Services
{
    public class MessageService : IMessageService
    {
        private readonly IProtectionRepository protectionRepository;
        private readonly ICacheRepository cacheRepository;
        private readonly ISecureKeyRepository secureKeyRepository;

        public MessageService(IProtectionRepository _protectionRepository, ICacheRepository _cacheRepository, ISecureKeyRepository _secureKeyRepository)
        {
            protectionRepository = _protectionRepository;
            cacheRepository = _cacheRepository;
            secureKeyRepository = _secureKeyRepository;
        }

        public async Task<string> Create(string message, int ttl)
        {
            try
            {
                // Generate storage key
                var key = secureKeyRepository.Create();

                // Protect string
                var protectedMessage = protectionRepository.Protect(message);

                // Save string to cache
                await cacheRepository.SaveAsync(key, protectedMessage, ttl);

                // Return token to be used in uri
                return key;
            }
            catch (Exception e)
            {
                throw new Exception("Could not cache message.", e);
            }
        }

        public async Task<bool> MessageExists(string key)
        {
            try
            {
                // Test to see if the message key exists in the cache
                return await cacheRepository.TestAsync(key);
            }
            catch (Exception e)
            {
                throw new Exception("Could not validate message.", e);
            }
        }

        public async Task<string> Retrieve(string key)
        {
            try
            {
                // Retrieve protected string from cache
                var protectedMessage = await cacheRepository.RetrieveAsync(key);

                // Encrypt string
                var unprotectedMessage = protectionRepository.Unprotect(protectedMessage);

                // Return the unprotected message
                return unprotectedMessage;
            }
            catch (Exception e)
            {
                throw new Exception("Could not retrieve message.", e);
            }
        }
    }
}
