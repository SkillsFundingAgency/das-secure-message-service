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
    public class HomeController : Controller
    {
        private readonly ILogger logger;
        private readonly IMessageService messageService;

        private readonly IOptions<SharedConfig> configuration;

        public HomeController(ILogger<HomeController> _logger, IMessageService _messageService, IOptions<SharedConfig> _configuration)
        {
            logger = _logger;
            messageService = _messageService;
            configuration = _configuration;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index", new IndexViewModel());
        }

        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexSubmitMessage(IndexViewModel indexViewModel)
        {
            if (String.IsNullOrEmpty(indexViewModel.Message))
            {
                logger.LogError(1, "Message cannot be null");
                return new BadRequestResult();
            }

            var key = await messageService.Create(indexViewModel.Message, indexViewModel.Ttl);
            logger.LogInformation(1, $"Saving message: {key}");

            return RedirectToAction("ShareMessageUrl", "Messages", new { key = key });
        }

        [AllowAnonymous]
        [HttpGet("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View("loggedOut");
        }

        [AllowAnonymous]
        [HttpGet("accessdenied")]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied", new AccessDeniedViewModel(configuration.Value.ValidOrganizations));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier });
        }
    }
}
