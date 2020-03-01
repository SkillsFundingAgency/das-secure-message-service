using Newtonsoft.Json;

namespace SFA.DAS.SecureMessageService.Api.Models
{
    public class GetSecureMessageResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}