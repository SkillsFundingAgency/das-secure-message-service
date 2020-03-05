using Moq;
using NUnit.Framework;
using SFA.DAS.SecureMessageService.Core.IRepositories;
using SFA.DAS.SecureMessageService.Infrastructure.Repositories;
using System;

namespace SFA.DAS.SecureMessageService.Infrastructure.UnitTests
{
    [TestFixture]
    public class ProtectionRepositoryTests
    {
        private Mock<IDasDataProtector> dataProtector;
        private ProtectionRepository repository;
        private string protectedMessage = "xxxxx";
        private string unprotectedMessage = "test message";

        [SetUp]
        public void Setup()
        {
            dataProtector = new Mock<IDasDataProtector>();
            repository = new ProtectionRepository(dataProtector.Object);
        }

        [Test]
        public void Protect_ProtectsAMessage()
        {
            // Arrange
            dataProtector.Setup(c => c.Protect(unprotectedMessage)).Returns(protectedMessage);

            // Act
            var result = repository.Protect(unprotectedMessage);

            // Assert
            Assert.AreEqual(protectedMessage, result);
            dataProtector.VerifyAll();
        }

        [Test]
        public void Protect_ThrowsAnException()
        {
            // Arrange
            dataProtector.Setup(c => c.Protect(unprotectedMessage)).Throws<Exception>();

            // Assert
            Assert.Throws<Exception>(() => repository.Protect(unprotectedMessage));
        }

        [Test]
        public void Unprotect_UnprotectsAMessage()
        {
            // Arrange
            dataProtector.Setup(c => c.Unprotect(protectedMessage)).Returns(unprotectedMessage);

            // Act
            var result = repository.Unprotect(protectedMessage);

            // Assert
            Assert.AreEqual(unprotectedMessage, result);
            dataProtector.VerifyAll();
        }

        [Test]
        public void Unprotect_ThrowsAnException()
        {
            // Arrange
            dataProtector.Setup(c => c.Unprotect(protectedMessage)).Throws<Exception>();

            // Assert
            Assert.Throws<Exception>(() => repository.Unprotect(protectedMessage));
        }
    }
}
