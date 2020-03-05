using SFA.DAS.SecureMessageService.Core.IRepositories;
using System;

namespace SFA.DAS.SecureMessageService.Infrastructure.Repositories
{
    public class ProtectionRepository : IProtectionRepository
    {
        private readonly IDasDataProtector dataProtector;

        public ProtectionRepository(IDasDataProtector _dataProtector)
        {
            dataProtector = _dataProtector;
        }

        public string Protect(string message)
        {
            try
            {
                return dataProtector.Protect(message);
            }
            catch (Exception e)
            {
                throw new Exception("Could not protect message", e);
            }
        }

        public string Unprotect(string message)
        {
            try
            {
                return dataProtector.Unprotect(message);
            }
            catch (Exception e)
            {
                throw new Exception("Could not unprotect message", e);
            }
        }
    }
}
