using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Web.Controllers;
using SFA.DAS.SecureMessageService.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.SecureMessageService.Web.UnitTests
{
    [TestFixture]
    public class MessagesControllerTests
    {
        protected Mock<ILogger<MessagesController>> logger;
        protected Mock<IMessageService> messageService;
        protected ControllerContext controllerContext;
        protected MessagesController controller;
        protected string testHttpScheme = "https";
        protected string testHostname = "localhost";
        protected string testKey = "36ae5a4a-068c-450a-9edf-f5f56d74f13e";
        protected string testMessage = "test message";
        protected int testPort = 1234;

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger<MessagesController>>();
            messageService = new Mock<IMessageService>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.SetupGet(t => t.Scheme).Returns(testHttpScheme);
            mockRequest.SetupGet(t => t.Host).Returns(new HostString(testHostname, testPort));
            mockRequest.SetupGet(t => t.Headers).Returns(new HeaderDictionary());
            mockHttpContext.SetupGet(h => h.Request).Returns(mockRequest.Object);
            controllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
            controller = new MessagesController(messageService.Object, logger.Object);
            controller.ControllerContext = controllerContext;
        }

        [Test]
        public async Task ShareMessageUrl_SuccessfullyRetrievesMessageUrlWhenMessageExistsWithGateway()
        {
            // Arrange
            var gatewayHost = "gatewayHost";

            var request = new Mock<HttpRequest>();
            var context = new Mock<HttpContext>();
            request.SetupGet(t => t.Scheme).Returns(testHttpScheme);
            request.SetupGet(t => t.Host).Returns(new HostString(gatewayHost, testPort));
            request.SetupGet(t => t.Headers).Returns(
                new HeaderDictionary {
                    {"X-Original-Host", gatewayHost}
                });
            context.SetupGet(t => t.Request).Returns(request.Object);
            controllerContext = new ControllerContext()
            {
                HttpContext = context.Object
            };
            controller = new MessagesController(messageService.Object, logger.Object);
            controller.ControllerContext = controllerContext;

            messageService.Setup(e => e.MessageExists(testKey)).ReturnsAsync(true);

            // Act
            var result = await controller.ShareMessageUrl(testKey);

            // Assert
            var actualResult = result as ViewResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(typeof(ShowMessageUrlViewModel), actualResult.Model.GetType());
            Assert.AreEqual(((ShowMessageUrlViewModel)actualResult.Model).Url, $"{testHttpScheme}://{gatewayHost}:{testPort}/messages/view/{testKey}");
            messageService.VerifyAll();
        }

        [Test]
        public async Task ShareMessageUrl_ReturnsInvalidMessageKeyViewWhenInvalidKeyIsPassed()
        {
            // Arrange
            messageService.Setup(e => e.MessageExists(testKey)).ReturnsAsync(false);

            // Act
            var result = await controller.ShareMessageUrl(testKey);

            // Assert
            var actualResult = result as ViewResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual("InvalidMessageKey", actualResult.ViewName);
            messageService.VerifyAll();
        }

        [Test]
        public async Task ConfirmViewMessage_DisplaysTheCorrectViewWhenRetrievingAMessageAndItDoesExist()
        {
            // Arrange
            messageService.Setup(e => e.MessageExists(testKey)).ReturnsAsync(true);

            // Act
            var result = await controller.ConfirmViewMessage(testKey);

            // Assert
            var actualResult = result as ViewResult;
            Assert.AreEqual("ConfirmViewMessage", actualResult.ViewName);
            Assert.AreEqual(true, actualResult.ViewData["MessageExists"]);
            messageService.VerifyAll();
        }

        [Test]
        public async Task ConfirmViewMessage_DisplaysTheCorrectViewWhenRetrievingAMessageAndItDoesNotExist()
        {
            // Arrange
            messageService.Setup(e => e.MessageExists(testKey)).ReturnsAsync(false);

            // Act
            var result = await controller.ConfirmViewMessage(testKey);

            // Assert
            var actualResult = result as ViewResult;
            Assert.AreEqual("ConfirmViewMessage", actualResult.ViewName);
            Assert.AreEqual(false, actualResult.ViewData["MessageExists"]);
            messageService.VerifyAll();
        }

        [Test]
        public async Task ViewMessage_DisplaysAMessageAfterConfirmationWhenItExists()
        {
            // Arrange
            messageService.Setup(e => e.Retrieve(testKey)).ReturnsAsync(testMessage);

            // Act
            var result = await controller.ViewMessage(testKey);

            // Assert
            var actualResult = result as ViewResult;
            Assert.AreEqual(typeof(ViewMessageViewModel), actualResult.Model.GetType());
            Assert.AreEqual(((ViewMessageViewModel)actualResult.Model).Message, testMessage);
            messageService.VerifyAll();
        }
    }
}
