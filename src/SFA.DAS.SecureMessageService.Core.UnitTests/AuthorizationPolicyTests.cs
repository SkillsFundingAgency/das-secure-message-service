using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.SecureMessageService.Core.Entities;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Core.Services;
using SFA.DAS.SecureMessageService.Core.IRepositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.SecureMessageService.Core.UnitTests
{
    [TestFixture]
    public class AuthorizationPolicyTests
    {
        private Mock<AuthorizationHandlerContext> context;
        private Mock<ValidOrganizationRequirement> requirement;

        [SetUp]
        public void Setup()
        {
            context = new Mock<AuthorizationHandlerContext>();
            requirement = new Mock<ValidOrganizationRequirement>();
        }

        [Test]
        public async Task HandleRequirementAsync_ReturnsSuccessFalseWhenClaimsDoNotExist()
        {
            // Arrange

            // Act

            // Assert
        }

        [Test]
        public async Task HandleRequirementAsync_ReturnsSuccessTrueWhenValidOrgsArePresent()
        {
            // Arrange

            // Act

            // Assert
        }

        [Test]
        public async Task HandleRequirementAsync_ReturnsSuccessFalseWhenClaimsExistButNoValidOrgsArePresent()
        {
            // Arrange

            // Act

            // Assert
        }

    }
}