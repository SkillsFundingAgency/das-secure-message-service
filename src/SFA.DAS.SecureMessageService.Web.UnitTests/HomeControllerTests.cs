using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.SecureMessageService.Core.Entities;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Web.Controllers;
using SFA.DAS.SecureMessageService.Web.Models;

namespace SFA.DAS.SecureMessageService.Web.UnitTests
{
    [TestFixture]
    public class HomeControllerTests
    {
        protected Mock<ILogger<HomeController>> logger;
        protected Mock<IMessageService> messageService;
        protected HomeController controller;
        protected IndexViewModel indexViewModel;
        protected string testMessage = "testmessage";
        protected int testTtl = 1;
        protected string testKey = "somekey1234";

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger<HomeController>>();
            messageService = new Mock<IMessageService>();
            controller = new HomeController(logger.Object, messageService.Object);
            indexViewModel = new IndexViewModel()
            {
                Message = testMessage,
                Ttl = testTtl
            };
        }

        [Test]
        public void Index_ReturnsExpectedViewResult()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            var actualViewResult = result as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual(actualViewResult.Model.GetType(), typeof(IndexViewModel));
            Assert.AreEqual("Index", actualViewResult.ViewName);
        }


        [Test]
        public async Task IndexSubmitMessage_SuccessfullyRedirectsOnFormPost()
        {
            // Arrange
            messageService.Setup(c => c.Create(testMessage, testTtl)).ReturnsAsync(testKey);

            // Act
            var result = await controller.IndexSubmitMessage(indexViewModel);

            // Assert
            Assert.IsNotNull(result);
            var actualResult = result as RedirectToActionResult;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual("ShareMessageUrl", actualResult.ActionName);
            Assert.AreEqual("Messages", actualResult.ControllerName);
            Assert.AreEqual(new RouteValueDictionary(new { key = testKey }), actualResult.RouteValues);
            messageService.VerifyAll();
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task IndexSubmitMessage_ReturnsBadRequestWhenNoMessage(string message)
        {
            // Arrange
            indexViewModel.Message = message;
            
            // Act
            var result = await controller.IndexSubmitMessage(indexViewModel);

            // Assert
            var actualResult = result as BadRequestResult;
            Assert.IsNotNull(actualResult);
        }

        [Test]
        public void Error_ReturnsExpectedViewResult()
        {
            // Act
            var result = controller.Error();

            // Assert
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            var actualViewResult = result as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual(actualViewResult.Model.GetType(), typeof(ErrorViewModel));
            Assert.AreEqual("Error", actualViewResult.ViewName);
        }
    }
}