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
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IMessageService messageService;
        private readonly ILogger logger;

        public MessagesController(IMessageService _messageService, ILogger<MessagesController> _logger)
        {
            messageService = _messageService;
            logger = _logger;
        }

        [HttpGet("Messages")]
        public IActionResult CreateMessage()
        {
            return View("CreateMessage", new CreateMessageViewModel());
        }

        [HttpPost("Messages")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostCreateMessage(CreateMessageViewModel createMessageViewModel)
        {
            if (String.IsNullOrEmpty(createMessageViewModel.Message))
            {
                logger.LogError(1, "Message cannot be null");
                return new BadRequestResult();
            }

            var key = await messageService.Create(createMessageViewModel.Message, createMessageViewModel.Ttl);
            logger.LogInformation(1, $"Saving message: {key}");

            return RedirectToAction("ShareMessageUrl", "Messages", new { key = key });
        }

        [HttpGet("Messages/Share/{key}")]
        public async Task<IActionResult> ShareMessageUrl(string key)
        {
            // Check for message in cache
            var messageExists = await messageService.MessageExists(key);
            if(!messageExists)
            {
                logger.LogError(1, $"Message with key {key} does not exist");
                return View("InvalidMessageKey");
            }

            // Create url and return view
            var url = $"{Request.Scheme}://{Request.Host}/Messages/{key}";
            var showMessageUrlViewModel = new ShowMessageUrlViewModel() { Url = url };
            return View("ShowMessageUrl", showMessageUrlViewModel);
        }

        [HttpGet("Messages/{key}")]
        public async Task<IActionResult> ConfirmViewMessage(string key)
        {
            var messageExists = await messageService.MessageExists(key);
            ViewBag.MessageExists = messageExists;
            logger.LogInformation(1, $"Message {key} exists: {messageExists.ToString()}");

            return View("ConfirmViewMessage");
        }

        [HttpPost("Messages/{key}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewMessage(string key)
        {
            logger.LogInformation(1, $"Attempting to retrieve message: {key}");
            var message = await messageService.Retrieve(key);
            logger.LogInformation(1, $"Message {key} has been removed from cache");

            var viewMessageViewModel = new ViewMessageViewModel() { Message = message };
            return View(viewMessageViewModel);
        }
    }
}
