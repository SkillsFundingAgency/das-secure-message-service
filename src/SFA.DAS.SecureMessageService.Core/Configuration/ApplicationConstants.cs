using System.Collections.Generic;

namespace SFA.DAS.SecureMessageService.Core.Configuration
{
    public static class ApplicationConstants
    {
        public const string ApplicationName = "das-tools-service";

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