using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.SecureMessageService.Core.IServices;
using Microsoft.Extensions.Logging;
using SFA.DAS.SecureMessageService.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Web.Http;

namespace SFA.DAS.SecureMessageService.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMessageService _messageService;
        private readonly IConfiguration _configuration;

        public MessagesController(IMessageService messageService, ILogger<MessagesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _messageService = messageService;
            _configuration = configuration;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateSecureMessageUrl([FromBody]SecureMessageRequestDto secureMessageRequest)
        {
            if (String.IsNullOrEmpty(secureMessageRequest.Message))
            {
                return BadRequest("The message parameter cannot be empty.");
            }

            if (!Enum.IsDefined(typeof(Ttl), secureMessageRequest.Ttl))
            {
                return BadRequest($"Ttl must be one of [{String.Join(",", Enum.GetNames(typeof(Ttl)))}]");
            }

            var ttl = (int)Enum.Parse(typeof(Ttl), secureMessageRequest.Ttl.ToString());
            var key = await _messageService.Create(secureMessageRequest.Message, ttl);
            _logger.LogInformation(1, $"Saving message: {key}");

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return Ok(new CreateSecureMessageResponse {

                Key = key,
                Links = new Links
                {
                    Api = $"{baseUrl}{Request.PathBase}/{key}",
                    Web = $"{baseUrl}/messages/view/{key}"
                }
            });
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetSecureMessage([FromRoute]string key)
        {
            if (String.IsNullOrEmpty(key)){
                return BadRequest("An identifier must be provided.");
            }

            if (! await _messageService.MessageExists(key))
            {
                return BadRequest("Invalid identifier.");
            }

            var message = await _messageService.Retrieve(key);
            return Ok(new GetSecureMessageResponse
            {
                Message = message
            });
        }
    }
}
