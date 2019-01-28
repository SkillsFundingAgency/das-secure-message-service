namespace SFA.DAS.SecureMessageService.Api.Models
{
    public class SecureMessageRequestDto
    {
        public string SecureMessage { get; set; }
        public int TtlInHours { get; set; }
    }
}