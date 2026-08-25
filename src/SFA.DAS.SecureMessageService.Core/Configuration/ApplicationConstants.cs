using System.Collections.Generic;

namespace SFA.DAS.SecureMessageService.Core.Configuration
{
    public static class ApplicationConstants
    {
        public const string ApplicationName = "das-tools-service";

        public const string DisabledAppInsightsConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000";

        public static Dictionary<int, string> TtlValues
        {
            get => new Dictionary<int, string>()
                {
                    { 1, "Hour" },
                    { 24, "Day" }
                };
        }
    }
}