using Microsoft.Extensions.Logging;
using SMS.App.Services.Strategies;
using SMS.App.Models.Entities;
using Moq;


namespace SMS.Tests.Strategies
{
    public class PingStrategyTests
    {
        private readonly Mock<ILogger<PingCheckStrategy>> _loggerMock;
        private readonly PingCheckStrategy _pingStrategy;

        public PingStrategyTests()
        {
            _loggerMock = new Mock<ILogger<PingCheckStrategy>>();
            _pingStrategy = new PingCheckStrategy(_loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldPass_WhenPingingExistingAddress()
        {
            // Arrange
            var target = new Target { URL = "127.0.0.1" };

            // Act
            var result = await _pingStrategy.ExecuteAsync(target);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldFail_WhenPingingNonExistingAddress()
        {
            // Arrange
            var target = new Target { URL = "256.256.256.256" };

            // Act
            var result = await _pingStrategy.ExecuteAsync(target);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
