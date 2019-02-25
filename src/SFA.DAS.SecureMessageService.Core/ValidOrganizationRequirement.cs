using Microsoft.AspNetCore.Authorization;

public class ValidOrganizationRequirement : IAuthorizationRequirement
{
    public string[] Organizations { get; private set; }

    public ValidOrganizationRequirement(string organizations)
    {
        Organizations = organizations.Split(",");
    }
}