using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Core.Entities;
using SFA.DAS.SecureMessageService.Web.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SFA.DAS.SecureMessageService.Web.Controllers
{
    [Authorize(Policy="ValidOrgsOnly")]
    public class AccountController : Controller
    {
        private readonly ILogger logger;

        private readonly IOptions<SharedConfig> configuration;

        public AccountController(ILogger<AccountController> _logger, IOptions<SharedConfig> _configuration)
        {
            logger = _logger;
            configuration = _configuration;
        }

        [AllowAnonymous]
        public async Task SignIn(string returnUrl = "/")
        {
            logger.LogInformation(1, $"A user has signed in.");
            await HttpContext.ChallengeAsync(new AuthenticationProperties() { RedirectUri = returnUrl});
        }

        [AllowAnonymous]
        public async Task<IActionResult> SignOut()
        {

            if (User.Identity.IsAuthenticated) {
                logger.LogInformation(1, $"A user has signed out.");
                var callbackUrl = Url.RouteUrl("SignedOut", "Account", null, Request.Scheme);

                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return RedirectToAction("SignedOut");
        }

        [AllowAnonymous]
        public IActionResult SignedOut()
        {
            logger.LogInformation(1, $"Redirecting user to signed out page.");
            return View("SignedOut");
        }

        [AllowAnonymous]
        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            var user = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "urn:github:login" && c.Issuer == "GitHub").Value;
            logger.LogInformation(1, $"The user {user} is not authorized to access this service. Ensure that they are a member of the correct organization.");
            return View("AccessDenied", new AccessDeniedViewModel(configuration.Value.ValidOrganizations));
        }
    }
}
