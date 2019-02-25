using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.SecureMessageService.Core.Entities;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Core.IRepositories;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class ValidOrganizationHandler : AuthorizationHandler<ValidOrganizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   ValidOrganizationRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == "urn:github:orgs" &&
                                        c.Issuer == "GitHub"))
        {
            return Task.CompletedTask;
        }

        var organizations =
            context.User.FindFirst(c => c.Type == "urn:github:orgs" &&
                                        c.Issuer == "GitHub").Value.Split(",");

        if (organizations.Intersect(requirement.Organizations).Any())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}