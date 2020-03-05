namespace SFA.DAS.SecureMessageService.Core.IRepositories
{
    public interface IProtectionRepository
    {
        string Protect(string message);

        string Unprotect(string message);
    }
}
