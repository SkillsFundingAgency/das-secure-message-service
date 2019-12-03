using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.SecureMessageService.Core.IServices;
using Microsoft.Extensions.Logging;
using SFA.DAS.SecureMessageService.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.SecureMessageService.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Messages")]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IMessageService messageService;
        private readonly IConfiguration configuration;

        public MessagesController(IMessageService _messageService, ILogger<MessagesController> _logger, IConfiguration _configuration)
        {
            logger = _logger;
            messageService = _messageService;
            configuration = _configuration;
        }

        [HttpPost]
        [Route("CreateSecureMessageUrl")]
        public async Task<IActionResult> CreateSecureMessageUrl([FromBody]SecureMessageRequestDto secureMessageRequest)
        {
            if (String.IsNullOrEmpty(secureMessageRequest.SecureMessage))
            {
                logger.LogError(1, "Message cannot be null");
                return new BadRequestResult();
            }

            var key = await messageService.Create(secureMessageRequest.SecureMessage, secureMessageRequest.TtlInHours);
            logger.LogInformation(1, $"Saving message: {key}");

            var baseUrl = configuration["UIBaseUrl"];
            baseUrl = baseUrl.TrimEnd('/');
            var url = $"{baseUrl}/view/{key}";
            return Ok(url);
        }
    }
}
