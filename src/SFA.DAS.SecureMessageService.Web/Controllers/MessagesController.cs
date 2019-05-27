using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Web.Models;

namespace SFA.DAS.SecureMessageService.Web.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IMessageService messageService;
        private readonly ILogger logger;

        public MessagesController(IMessageService _messageService, ILogger<MessagesController> _logger)
        {
            messageService = _messageService;
            logger = _logger;
        }

        [HttpGet("share/{key}")]
        public async Task<IActionResult> ShareMessageUrl(string key)
        {
            // Check for message in cache
            var messageExists = await messageService.MessageExists(key);
            if(!messageExists)
            {
                logger.LogError($"Message with key {key} does not exist");
                return View("InvalidMessageKey");
            }

            // Create url and return view
            var url = $"{Request.Scheme}://{Request.Host}/messages/{key}";
            var showMessageUrlViewModel = new ShowMessageUrlViewModel() { Url = url };
            return View("ShowMessageUrl", showMessageUrlViewModel);
        }

        [HttpGet("messages/{key}")]
        public async Task<IActionResult> ConfirmViewMessage(string key)
        {
            var messageExists = await messageService.MessageExists(key);
            ViewBag.MessageExists = messageExists;
            logger.LogInformation($"Message {key} exists: {messageExists.ToString()}");

            return View("ConfirmViewMessage");
        }


        [HttpPost("messages/{key}")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewMessage(string key)
        {
            logger.LogInformation($"Attempting to retrieve message: {key}");
            var message = await messageService.Retrieve(key);
            logger.LogInformation($"Message {key} has been removed from cache");

            var viewMessageViewModel = new ViewMessageViewModel() { Message = message };
            return View(viewMessageViewModel);
        }
    }
}
