namespace SFA.DAS.SecureMessageService.Api.Configuration
{
    public static class ApiConstants
    {
        public const string ApiName = "Secure Message Service Api";
        public const string PathBase = "/api/messages";
        public const string ScopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";
        public const string ObjectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string AuthorizationPolicyName = "RequireMessageRole";
        public const string AuthorizationRequiredRoleName = "Messages";
    }
}
