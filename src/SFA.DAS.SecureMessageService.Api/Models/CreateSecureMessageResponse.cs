namespace SFA.DAS.SecureMessageService.Api.Models
{
    public class CreateSecureMessageResponse
    {
        public string Key { get; set; }

        public Links Links { get; set; }
    }

    public partial class Links
    {
        public string Api { get; set; }

        public string Web { get; set; }
    }
}
