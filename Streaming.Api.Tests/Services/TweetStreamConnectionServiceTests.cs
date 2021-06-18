namespace Streaming.Api.Tests.Services
{
    using System;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Implementation.Services;
    using Streaming.Api.Tests.Data;
    using Xunit;

    public class TweetStreamConnectionServiceTests
    {
        [Fact]
        public void ConnectToSampledStreamAsync_WithInvalidConnectionSettingKey_ThrowsException()
        {
            // Arrange
            var tweetProcessor = new Mock<ITweetProcessor>();
            var loggerStub = new NullLogger<TweetStreamConnectionService>();
            var configStub = FakeConfigurationProvider.BuildDefaultConfiguration();
            var apiEnv = new Mock<IApiEnvironment>();

            var dut = new TweetStreamConnectionService(
                loggerStub,
                tweetProcessor.Object,
                configStub,
                apiEnv.Object);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await dut.ConnectToSampledStreamAsync());
        }
    }
}
