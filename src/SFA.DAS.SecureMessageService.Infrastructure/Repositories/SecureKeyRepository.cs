using SFA.DAS.SecureMessageService.Core.IRepositories;
using System;
using System.Security.Cryptography;

namespace SFA.DAS.SecureMessageService.Infrastructure.Repositories
{
    public class SecureKeyRepository : ISecureKeyRepository
    {
        public string Create()
        {
            using (var provider = RandomNumberGenerator.Create())
            {
                var bytes = new byte[16];
                provider.GetBytes(bytes);
                bytes[8] = (byte)(bytes[8] & 0xBF | 0x80);
                bytes[7] = (byte)(bytes[7] & 0x4F | 0x40);
                return new Guid(bytes).ToString();
            }
        }
    }
}
