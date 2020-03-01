namespace SFA.DAS.SecureMessageService.Core.IRepositories
{
    public interface IDasDataProtector
    {
        string Protect(string message);

        string Unprotect(string message);
    }
}
