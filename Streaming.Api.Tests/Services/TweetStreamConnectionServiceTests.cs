namespace Streaming.Api.Tests.Services
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using RestSharp;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Implementation.Services;
    using Xunit;

    public class TweetStreamConnectionServiceTests
    {
        [Fact]
        public void ConnectAsync_WithInvalidConnectionSettingKey_ThrowsException()
        {
            // Arrange
            var tweetProcessor = new Mock<ITweetProcessor>();
            var loggerStub = new NullLogger<TweetStreamConnectionService>();
            var configStub = new Mock<IConfiguration>();

            var dut = new TweetStreamConnectionService(
                loggerStub,
                tweetProcessor.Object,
                configStub.Object);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await dut.ConnectToSampledStreamAsync());
        }
    }
}
