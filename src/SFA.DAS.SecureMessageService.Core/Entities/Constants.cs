using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.SecureMessageService.Core.Entities
{
    [ExcludeFromCodeCoverage]
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