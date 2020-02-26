using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.SecureMessageService.Core.IServices;
using SFA.DAS.SecureMessageService.Core.Services;
using SFA.DAS.SecureMessageService.Core.IRepositories;
using System.Threading.Tasks;

namespace SFA.DAS.SecureMessageService.Core.UnitTests
{
    [TestFixture]
    public class MessageServiceTests
    {
        private Mock<IProtectionRepository> protectionRepository;
        private Mock<ICacheRepository> cacheRepository;
        private Mock<ISecureKeyRepository> secureKeyRepository;
        private MessageService service;
        private string unprotectedMessage = "test message";
        private string protectedMessage = "xxxxxxxxxx";
        private string key = "24a8d272-0bd5-422d-80f1-09fc21dc7f7f";
        private int ttl = 1;

        [SetUp]
        public void Setup()
        {
            protectionRepository = new Mock<IProtectionRepository>();
            cacheRepository = new Mock<ICacheRepository>();
            secureKeyRepository = new Mock<ISecureKeyRepository>();

            service = new MessageService(protectionRepository.Object, cacheRepository.Object, secureKeyRepository.Object);
        }

        [Test]
        public async Task Create_ReturnsAValidKey()
        {
            // Arrange
            Guid returnedKey;
            protectionRepository.Setup(c => c.Protect(unprotectedMessage)).Returns(protectedMessage);
            cacheRepository.Setup(c => c.SaveAsync(It.IsAny<string>(), protectedMessage, ttl)).Returns(Task.FromResult(false));
            secureKeyRepository.Setup(c => c.Create()).Returns(key);

            // Act
            var result = await service.Create(unprotectedMessage, ttl);

            // Assert
            Assert.IsTrue(Guid.TryParse(result, out returnedKey));
            protectionRepository.VerifyAll();
            cacheRepository.VerifyAll();
            secureKeyRepository.VerifyAll();
        }

        [Test]
        public async Task Create_DoesNotReturnAValidKey()
        {
            // Arrange
            Guid returnedKey;
            protectionRepository.Setup(c => c.Protect(unprotectedMessage)).Returns(protectedMessage);
            cacheRepository.Setup(c => c.SaveAsync(It.IsAny<string>(), protectedMessage, ttl)).Returns(Task.FromResult(false));
            secureKeyRepository.Setup(c => c.Create()).Returns(key);

            // Act
            var result = await service.Create(unprotectedMessage, ttl);
            var incorrectResult = "incorrect format";

            // Assert
            Assert.IsFalse(Guid.TryParse(incorrectResult, out returnedKey));
            protectionRepository.VerifyAll();
            cacheRepository.VerifyAll();
            secureKeyRepository.VerifyAll();

        }

        [Test]
        public void Create_ThrowsAValidException()
        {
            // Arrange
            protectionRepository.Setup(c => c.Protect(unprotectedMessage)).Throws<Exception>();
            cacheRepository.Setup(c => c.SaveAsync(It.IsAny<string>(), protectedMessage, ttl)).Returns(Task.FromResult(false));
            secureKeyRepository.Setup(c => c.Create()).Returns(key);

             // Assert
            Assert.ThrowsAsync<Exception>(async () => await service.Create(unprotectedMessage, ttl));
        }

        [Test]
        public async Task MessageExists_ReturnsTrueIfMessageIsPresent()
        {
            //Arrange
            var key = Guid.NewGuid().ToString();
            cacheRepository.Setup(c => c.TestAsync(key)).ReturnsAsync(true);

            // Act
            var result = await service.MessageExists(key);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task MessageExists_ReturnsFalseIfMessageDoesNotExistInCache()
        {
            //Arrange
            cacheRepository.Setup(c => c.TestAsync(key)).ReturnsAsync(false);

            // Act
            var result = await service.MessageExists(key);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MessageExists_ThrowsAValidException()
        {
            //Arrange
            cacheRepository.Setup(c => c.TestAsync(key)).Throws<Exception>();

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await service.MessageExists(key));
        }

        [Test]
        public async Task Retrieve_ReturnsAValidMessage()
        {
            // Arrange
            cacheRepository.Setup(c => c.RetrieveAsync(key)).ReturnsAsync(protectedMessage);
            protectionRepository.Setup(c => c.Unprotect(protectedMessage)).Returns(unprotectedMessage);

            // Act
            var result = await service.Retrieve(key);

            // Assert
            Assert.AreEqual(unprotectedMessage, result);
            cacheRepository.VerifyAll();
            protectionRepository.VerifyAll();
        }

        [Test]
        public async Task Retrieve_ReturnsAnInvalidMessage()
        {
            // Arrange
            cacheRepository.Setup(c => c.RetrieveAsync(key)).ReturnsAsync(protectedMessage);
            protectionRepository.Setup(c => c.Unprotect(protectedMessage)).Returns(default(String));

            // Act
            var result = await service.Retrieve(key);

            // Assert
            Assert.AreNotEqual(unprotectedMessage, result);
            cacheRepository.VerifyAll();
            protectionRepository.VerifyAll();
        }

        [Test]
        public void Retrieve_ThrowsAValidException()
        {
            // Arrange
            cacheRepository.Setup(c => c.RetrieveAsync(key)).ReturnsAsync(protectedMessage);
            protectionRepository.Setup(c => c.Unprotect(protectedMessage)).Throws<Exception>();

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await service.Retrieve(key));
            cacheRepository.VerifyAll();
            protectionRepository.VerifyAll();
        }
    }
}
