using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.SecureMessageService.Api.Controllers;
using SFA.DAS.SecureMessageService.Api.Models;
using SFA.DAS.SecureMessageService.Core.IServices;
using System.Threading.Tasks;

namespace SFA.DAS.SecureMessageService.Api.UnitTests
{
    [TestFixture]
    public class MessagesControllerTests
    {
        protected Mock<ILogger<MessagesController>> logger;
        protected Mock<IMessageService> messageService;
        protected Mock<IConfiguration> configuration;
        protected MessagesController controller;
        protected string testHttpScheme = "https";
        protected string testHostname = "localhost";
        protected string testKey = "36ae5a4a-068c-450a-9edf-f5f56d74f13e";
        protected string testMessage = "test message";
        protected int testTtl = 1;

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger<MessagesController>>();
            messageService = new Mock<IMessageService>();
            configuration = new Mock<IConfiguration>();
            configuration.SetupGet(t => t["UIBaseUrl"]).Returns($"{testHttpScheme}://{testHostname}");
            controller = new MessagesController(messageService.Object, logger.Object, configuration.Object);
        }

        [Test]
        public async Task SuccessfullyGeneratesSecretUrl()
        {
            //Arrange
            var request = new SecureMessageRequestDto
            {
                SecureMessage = testMessage,
                TtlInHours = testTtl
            };
            messageService.Setup(e => e.Create(testMessage, testTtl)).ReturnsAsync(testKey);


            //Act
            var result = await controller.CreateSecureMessageUrl(request);
            var actualResult = result as OkObjectResult;

            //Assert
            Assert.AreEqual($"{testHttpScheme}://{testHostname}/view/{testKey}", actualResult.Value.ToString());
            messageService.VerifyAll();
        }
    }
}
