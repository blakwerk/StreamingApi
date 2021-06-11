namespace Streaming.Api.Tests.Services
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using RestSharp;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Implementation.Services;
    using Xunit;

    public class TweetStreamConnectionServiceTests
    {
        [Fact]
        public void ConnectAsync_WithInvalidConnectionSettingKey_ThrowsException()
        {
            // Arrange
            var clientMock = new Mock<IClientFactory>();
            var loggerStub = new NullLogger<TweetStreamConnectionService>();
            var configStub = new Mock<IConfiguration>();

            var dut = new TweetStreamConnectionService(
                loggerStub,
                clientMock.Object,
                configStub.Object);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await dut.ConnectToSampledStreamAsync());
        }
    }
}
