using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.SecureMessageService.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class SecureMessageRequestDto
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("ttl")]
        public string Ttl { get; set; }
    }

    public enum Ttl
    {
        Hour = 1,
        Day = 24
    }
}


