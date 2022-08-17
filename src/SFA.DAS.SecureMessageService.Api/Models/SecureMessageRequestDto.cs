using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.SecureMessageService.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class SecureMessageRequestDto
    {
        public string Message { get; set; }

        public string Ttl { get; set; }
    }

    public enum Ttl
    {
        Hour = 1,
        Day = 24
    }
}
