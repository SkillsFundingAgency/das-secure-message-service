using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
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
        protected string testMessage = "testmessage";
        protected int testTtl = 1;
        protected string testKey = "somekey1234";

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger<HomeController>>();
            messageService = new Mock<IMessageService>();
            controller = new HomeController(logger.Object, messageService.Object);
            var controllerContext = new Mock<ControllerContext>();
        }

        [Test]
        public void Index_ReturnsExpectedResultWhenUserIsNotAuthenticated()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()

            };
            // Act
            var result = controller.Index();

            // Assert
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            var actualViewResult = result as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual("Index", actualViewResult.ViewName);
        }

        [Test]
        public void Index_ReturnsExpectedResultWhenUserIsAuthenticated()
        {
            // Arrange
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "username")
                    }, "someAuthTypeName"))
                }
            };

            // Act
            var result = controller.Index();

            // Assert
            Assert.AreEqual(typeof(RedirectToActionResult), result.GetType());
            var actualRedirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(actualRedirectToActionResult);
            Assert.AreEqual("CreateMessage", actualRedirectToActionResult.ActionName);
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