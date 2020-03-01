using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.SecureMessageService.Api.Controllers;
using SFA.DAS.SecureMessageService.Api.Models;
using SFA.DAS.SecureMessageService.Core.IServices;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.SecureMessageService.Api.UnitTests
{
    [TestFixture]
    public class MessagesControllerTests
    {
        protected Mock<IMessageService> _messageService;
        protected Mock<IConfiguration> _configuration;
        protected MessagesController _controller;
        protected SecureMessageRequestDto _secureMessageRequestDto;
        protected string _testHttpScheme = "https";
        protected string _testHostname = "localhost";
        protected string _testPathBase = "/api/messages";
        protected string _testKey = Guid.NewGuid().ToString();
        protected string _testMessage = "test message";
        protected string _testTtl = "Hour";
        protected string _baseUrl;

        [SetUp]
        public void Setup()

        {
            _messageService = new Mock<IMessageService>();
            _configuration = new Mock<IConfiguration>();

            _baseUrl = $"{_testHttpScheme}://{_testHostname}";

            _controller = new MessagesController(_messageService.Object, Mock.Of<ILogger<MessagesController>>(), _configuration.Object);
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Scheme = _testHttpScheme;
            _controller.ControllerContext.HttpContext.Request.Host = new HostString(_testHostname);
            _controller.ControllerContext.HttpContext.Request.PathBase = new PathString(_testPathBase);
        }

        [Test]
        public async Task SuccessfullyGeneratesSecretUrl()
        {
            //Arrange
            var secureMessageRequestDto = new SecureMessageRequestDto
            {
                Message = _testMessage,
                Ttl = _testTtl
            };

            var createSecureMessageResponse = new CreateSecureMessageResponse
            {
                Key = _testKey,
                Links = new Links
                {
                    Api = $"{_baseUrl}{_testPathBase}/{_testKey}",
                    Web = $"{_baseUrl}/messages/view/{_testKey}"
                }
            };

            _messageService.Setup(e => e.Create(_testMessage, 1)).ReturnsAsync(_testKey);

            //Act
            var result = await _controller.CreateSecureMessageUrl(secureMessageRequestDto);
            var actualResult = result as OkObjectResult;

            //Assert
            _messageService.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(actualResult.Value);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)actualResult.StatusCode);
            Assert.IsInstanceOf<CreateSecureMessageResponse>(actualResult.Value);
            var secureMessageResponse = actualResult.Value as CreateSecureMessageResponse;
            Assert.AreEqual(createSecureMessageResponse.Key, secureMessageResponse.Key);
            Assert.AreEqual(createSecureMessageResponse.Links.Api, secureMessageResponse.Links.Api);
            Assert.AreEqual(createSecureMessageResponse.Links.Web, secureMessageResponse.Links.Web);
        }

        [Test]
        public async Task SuccesfullyRetrieveSecretByKey()
        {
            //Arrange
            _messageService.Setup(e => e.MessageExists(_testKey)).ReturnsAsync(true);
            _messageService.Setup(e => e.Retrieve(_testKey)).ReturnsAsync(_testMessage);

            var getSecureMessageResponse = new GetSecureMessageResponse
            {
                Message = _testMessage
            };

            //Act
            var result = await _controller.GetSecureMessage(_testKey);
            var actualResult = result as OkObjectResult;

            //Assert
            _messageService.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(actualResult.Value);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)actualResult.StatusCode);
            Assert.IsInstanceOf<GetSecureMessageResponse>(actualResult.Value);
            var secureMessageResponse = actualResult.Value as GetSecureMessageResponse;
            Assert.AreEqual(getSecureMessageResponse.Message, secureMessageResponse.Message);
        }
    }
}
