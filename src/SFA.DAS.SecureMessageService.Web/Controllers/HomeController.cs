using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Web.Models;

namespace SFA.DAS.SecureMessageService.Web.Controllers
{
    [Authorize(Policy="ValidOrgsOnly")]
    public class HomeController : Controller
    {
        private readonly ILogger logger;
        private readonly IMessageService messageService;

        public HomeController(ILogger<HomeController> _logger, IMessageService _messageService)
        {
            logger = _logger;
            messageService = _messageService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index", new IndexViewModel());
        }

        [HttpPost("")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexSubmitMessage(IndexViewModel indexViewModel)
        {
            if (String.IsNullOrEmpty(indexViewModel.Message))
            {
                logger.LogError("Message cannot be null");
                return new BadRequestResult();
            }

            var key = await messageService.Create(indexViewModel.Message, indexViewModel.Ttl);
            logger.LogInformation($"Saving message: {key}");

            return RedirectToAction("ShareMessageUrl", "Messages", new { key = key });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier });
        }
    }
}
