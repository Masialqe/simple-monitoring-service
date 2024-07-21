using Microsoft.Extensions.Logging;
using SMS.App.Services.Strategies;
using SMS.App.Models.Entities;
using Moq;

namespace SMS.Tests.Strategies
{
    public class HttpCheckStrategyTests
    {
        private readonly Mock<ILogger<HttpCheckStrategy>> _loggerMock;
        private readonly Mock<HttpClient> _httpClientMock;  
        private readonly HttpCheckStrategy _httpStrategy;

        public HttpCheckStrategyTests()
        {
            _httpClientMock = new Mock<HttpClient>();
            _loggerMock = new Mock<ILogger<HttpCheckStrategy>>();

            _httpStrategy = new HttpCheckStrategy(_httpClientMock.Object, 
                _loggerMock.Object);   
        }

        [Fact]
        public async Task ExecuteAsync_ShouldPass_WhenGivenAddressDoesExists()
        {
            //Arrange
            var target = new Target { URL = "https://www.google.pl/" };

            //Act
            var result = await _httpStrategy.ExecuteAsync(target);

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNotPass_WhenGivenAddressDoesntExists()
        {
            //Arrange
            var target = new Target { URL = "http://noixnsa8hx89axjsaxsaxsa.xsok" };

            //Act
            var result = await _httpStrategy.ExecuteAsync(target);

            //Assert
            Assert.False(result.IsSuccess);
        }
    }
}
