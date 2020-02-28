using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.SecureMessageService.Api.Models
{
    public class GetSecureMessageResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
