using System;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Caching.Distributed;
using SFA.DAS.SecureMessageService.Infrastructure.Repositories;
using System.Threading.Tasks;
using SFA.DAS.SecureMessageService.Core.IRepositories;

namespace SFA.DAS.SecureMessageService.Infrastructure.UnitTests
{
    [TestFixture]
    public class CacheRepositoryTests
    {
        private Mock<IDasDistributedCache> cache;
        private CacheRepository repository;
        private string message = "test message";
        private string key = "24a8d272-0bd5-422d-80f1-09fc21dc7f7f";
        private int ttl = 1;

        [SetUp]
        public void Setup()
        {
            cache = new Mock<IDasDistributedCache>();
            repository = new CacheRepository(cache.Object);
        }

        [Test]
        public async Task SaveAsync_CompletesSuccesfully()
        {
            // Arrange
            cache.Setup(c => c.SetStringAsync(key, message, It.IsAny<DistributedCacheEntryOptions>())).Returns(Task.FromResult(false));

            // Act
            await repository.SaveAsync(key, message, ttl);

            // Assert
            cache.VerifyAll();
        }

        [Test]
        public async Task RetrieveAsync_ReturnsAValidMessage()
        {
            // Arrange
            cache.Setup(c => c.GetStringAsync(key)).ReturnsAsync(message);
            cache.Setup(c => c.RemoveAsync(key)).Returns(Task.FromResult(false));

            // Act
            var result = await repository.RetrieveAsync(key);

            // Assert
            Assert.AreEqual(message, result);
            cache.VerifyAll();
        }

        [Test]
        public async Task RetrieveAsync_DoesNotReturnAValidMessage()
        {
            // Arrange
            cache.Setup(c => c.GetStringAsync(key)).ReturnsAsync(default(string));
            cache.Setup(c => c.RemoveAsync(key)).Returns(Task.FromResult(false));

            // Act
            var result = await repository.RetrieveAsync(key);

            // Assert
            Assert.AreEqual(null, result);
            cache.VerifyAll();
        }

        [Test]
        public async Task TestAsync_ReturnTrueIfMessageExistsInCache()
        {
            // Arrange
            cache.Setup(c => c.GetStringAsync(key)).ReturnsAsync(message);

            // Act
            var result = await repository.TestAsync(key);

            // Assert
            Assert.IsTrue(result);
            cache.VerifyAll();
        }

        [Test]
        public async Task TestAsync_ReturnFalseIfMessageDoesNotExistsInCache()
        {
            // Arrange
            cache.Setup(c => c.GetStringAsync(key)).ReturnsAsync(default(String));

            // Act
            var result = await repository.TestAsync(key);

            // Assert
            Assert.IsFalse(result);
            cache.VerifyAll();
        }
    }

}