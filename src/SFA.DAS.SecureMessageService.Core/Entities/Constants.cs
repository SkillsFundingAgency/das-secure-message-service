using System.Collections.Generic;

namespace SFA.DAS.SecureMessageService.Core.Entities
{
    public class Constants
    {
        public static Dictionary<int, string> TtlValues
        {
            get => new Dictionary<int, string>()
                {
                    { 1, "Hour" },
                    { 24, "Day" },
                    { 168, "Week" }
                };
        }
    }
}