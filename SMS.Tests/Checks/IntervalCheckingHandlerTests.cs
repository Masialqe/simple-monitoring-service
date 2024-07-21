using Microsoft.Extensions.Logging;
using SMS.App.Services.Strategies;
using SMS.App.Models.Entities;
using SMS.App.Models.Results;
using SMS.App.Handlers;
using Moq;

namespace SMS.Tests.Checks
{
    public class IntervalCheckingHandlerTests
    {
        private readonly Mock<ICheckStrategy> _httpStrategyMock;
        private readonly Mock<ICheckStrategy> _pingStrategyMock;
        private readonly Mock<ILogger<IntervalCheckingHandler>> _loggerMock;
        private readonly IntervalCheckingHandler _handler;

        public IntervalCheckingHandlerTests()
        {
            _httpStrategyMock = new Mock<ICheckStrategy>();
            _pingStrategyMock = new Mock<ICheckStrategy>();
            _loggerMock = new Mock<ILogger<IntervalCheckingHandler>>();

            _handler = new IntervalCheckingHandler(
                _httpStrategyMock.Object,
                _pingStrategyMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CheckAvailability_ShouldAddToFailedChecks_WhenResultIsFailureAndNotAlreadyFailed()
        {
            // Arrange
            var target = new Target { URL = "http://exaxsaxsaxsaxmple.com" };
            var result = new CheckResult { IsSuccess = false, Target = target };

            _httpStrategyMock
                .Setup(x => x.ExecuteAsync(It.IsAny<Target>()))
                .ReturnsAsync(result);

            // Act
            await _handler.CheckAvailability(target);

            // Assert
            Assert.Contains(result, _handler.FailedChecks);
        }

        [Fact]
        public async Task CheckAvailability_ShouldRemoveFromFailedChecks_WhenResultIsSuccessAndAlreadyFailed()
        {
            // Arrange
            var target = new Target { URL = "http://example.com" };
            var result = new CheckResult { IsSuccess = true, Target = target };

            _handler.FailedChecks.Add(new CheckResult { IsSuccess = false, Target = target });

            _httpStrategyMock
                .Setup(x => x.ExecuteAsync(It.IsAny<Target>()))
                .ReturnsAsync(result);

            // Act
            await _handler.CheckAvailability(target);

            // Assert
            Assert.DoesNotContain(result, _handler.FailedChecks);
        }

        [Fact]
        public async Task CheckAvailability_ShouldNotAddToFailedChecks_WhenResultIsFailureAndAlreadyFailed()
        {
            // Arrange
            var target = new Target { URL = "http://example.com" };
            var result = new CheckResult { IsSuccess = false, Target = target };

            _handler.FailedChecks.Add(result);

            _httpStrategyMock
                .Setup(x => x.ExecuteAsync(It.IsAny<Target>()))
                .ReturnsAsync(result);

            // Act
            await _handler.CheckAvailability(target);

            // Assert
            Assert.Equal(1, _handler.FailedChecks.Count);
        }

        [Fact]
        public async Task CheckAvailability_ShouldNotRemoveFromFailedChecks_WhenResultIsSuccessAndNotAlreadyFailed()
        {
            // Arrange
            var target = new Target { URL = "http://example.com" };
            var result = new CheckResult { IsSuccess = true, Target = target };

            _httpStrategyMock
                .Setup(x => x.ExecuteAsync(It.IsAny<Target>()))
                .ReturnsAsync(result);

            // Act
            await _handler.CheckAvailability(target);

            // Assert
            Assert.Empty(_handler.FailedChecks);
        }
    }
}
