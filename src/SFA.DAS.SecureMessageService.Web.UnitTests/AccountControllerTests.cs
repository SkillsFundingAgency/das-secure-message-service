using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.SecureMessageService.Core.Entities;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Web.Controllers;
using SFA.DAS.SecureMessageService.Web.Models;

namespace SFA.DAS.SecureMessageService.Web.UnitTests
{
    [TestFixture]
    public class AccountControllerTests
    {
        protected Mock<ILogger<AccountController>> logger;
        protected Mock<IOptions<SharedConfig>> configuration;
        protected ControllerContext controllerContext;
        protected AccountController controller;
        protected AccessDeniedViewModel accessDeniedViewModel;
        protected string testHttpScheme = "https";
        protected string testHostname = "localhost";
        protected int testPort = 1234;


        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger<AccountController>>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.SetupGet(t => t.Scheme).Returns(testHttpScheme);
            mockRequest.SetupGet(t => t.Host).Returns(new HostString(testHostname, testPort));
            mockHttpContext.SetupGet(h => h.Request).Returns(mockRequest.Object);
            controllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
            controller = new AccountController(logger.Object, configuration.Object);
            controller.ControllerContext = controllerContext;
        }

        [Test]
        public async Task SignIn_SuccessfullySignsInAUser()
        {
            // Arrange

            // Act
            await controller.SignIn();

            // Assert
        }

        [Test]
        public async Task SignOut_SuccessfullySignsOutaUser()
        {
            // Arrange

            // Act
            var result = await controller.SignOut();

            // Assert
            var actualResult = result as ViewResult;
            Assert.AreEqual("SignOut", actualResult.ViewName);
        }

        [Test]
        public void AccessDenied_RedirectsUserIfOrgMembershipIsNotValid()
        {
            // Arrange

            // Act
            var result = controller.AccessDenied();

            // Assert
            var actualResult = result as ViewResult;
            Assert.AreEqual("AccessDenied", actualResult.ViewName);
            Assert.AreEqual(((AccessDeniedViewModel)actualResult.Model).Organizations, "");
        }
    }
}