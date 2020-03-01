using Newtonsoft.Json;

namespace SFA.DAS.SecureMessageService.Api.Models
{
    public class CreateSecureMessageResponse
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }
    }

    public partial class Links
    {
        [JsonProperty("api")]
        public string Api { get; set; }

        [JsonProperty("web")]
        public string Web { get; set; }
    }
}