using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.SecureMessageService.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class SecureMessageRequestDto
    {
        public string SecureMessage { get; set; }
        public int TtlInHours { get; set; }
    }
}