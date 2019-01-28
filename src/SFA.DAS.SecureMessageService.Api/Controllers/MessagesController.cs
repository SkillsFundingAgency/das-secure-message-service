using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.SecureMessageService.Core.Entities;
using SFA.DAS.SecureMessageService.Core.IServices;
using Microsoft.Extensions.Logging;
using SFA.DAS.SecureMessageService.Api.Models;

namespace SFA.DAS.SecureMessageService.Api.Controllers
{
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IMessageService messageService;

        public MessagesController(ILogger<MessagesController> _logger, IMessageService _messageService)
        {
            logger = _logger;
            messageService = _messageService;
        }

        [HttpPost]
        [Route("CreateSecureMessageUrl")]
        public async Task<ActionResult> CreateSecureMessageUrl([FromBody]SecureMessageRequestDto secureMessageRequest)
        {
            if (String.IsNullOrEmpty(secureMessageRequest.SecureMessage))
            {
                logger.LogError(1, "Message cannot be null");
                return new BadRequestResult();
            }

            var key = await messageService.Create(secureMessageRequest.SecureMessage, secureMessageRequest.TtlInHours);
            logger.LogInformation(1, $"Saving message: {key}");

            var url = $"{Request.Scheme}://{Request.Host}/messages/{key}";
            return Ok(url);
        }
    }
}
