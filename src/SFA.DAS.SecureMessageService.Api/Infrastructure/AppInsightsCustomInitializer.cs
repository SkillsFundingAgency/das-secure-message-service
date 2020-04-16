using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace SFA.DAS.SecureMessageService.Api.Infrastructure
{
    public class ToolsTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                telemetry.Context.Cloud.RoleName = "das-secure-message-service-api";
                //telemetry.Context.Cloud.RoleInstance = "Custom RoleInstance";
            }
        }
    }
}
