namespace Streaming.Api.Tests.Data
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Implementation.Data;
    using Streaming.Api.Models;
    using Xunit;

    public class DataServiceTests
    {
        [Fact]
        public void ConnectAsync_WithInvalidConnectionSettingKey_ThrowsArgumentException()
        {
            // Arrange
            var environmentMock = new Mock<IApiEnvironment>();
            var loggerStub = new NullLogger<DataService>();
            var configStub = new Mock<IConfiguration>();

            var dut = new DataService(
                loggerStub,
                configStub.Object,
                environmentMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await dut.ConnectAsync());
        }

        [Fact]
        public void ConnectAsync_NullOrEmptyConnection_ThrowsArgumentException()
        {
            // Arrange
            var environmentMock = new Mock<IApiEnvironment>();
            var loggerStub = new NullLogger<DataService>();
            var configStub = new Mock<IConfiguration>();

            environmentMock.SetupGet(p => p.DatabaseConnection)
                .Returns(FakeConfigurationProvider.ValidKey);

            var dut = new DataService(
                loggerStub,
                configStub.Object,
                environmentMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await dut.ConnectAsync());
        }

        [Fact]
        public void ConnectAsync_ValidConnectionString_ConnectsToDb()
        {
            // Arrange
            var environmentMock = new Mock<IApiEnvironment>();
            var loggerStub = new NullLogger<DataService>();
            var fakeConfig = FakeConfigurationProvider.BuildDefaultConfiguration();

            environmentMock.SetupGet(p => p.DatabaseConnection)
                .Returns(FakeConfigurationProvider.ValidKey);

            var dut = new DataService(
                loggerStub,
                fakeConfig,
                environmentMock.Object);

            // Act
            dut.ConnectAsync().Wait();

            // Assert

            // For the purposes of this app, if we get to here, then we've successfully
            // connected to the db.
        }

        [Fact]
        public void ConnectAsync_DoesNotReconnectWhenConnected()
        {
            // Arrange
            var environmentMock = new Mock<IApiEnvironment>();
            var loggerStub = new NullLogger<DataService>();
            var fakeConfig = FakeConfigurationProvider.BuildDefaultConfiguration();

            environmentMock.SetupGet(p => p.DatabaseConnection)
                .Returns(FakeConfigurationProvider.ValidKey);

            var dut = new DataService(
                loggerStub,
                fakeConfig,
                environmentMock.Object);

            // Act
            dut.ConnectAsync().Wait();
            dut.ConnectAsync().Wait();

            // Assert
            // Ideally, if IDataStore were a wrapper, then verify the underlying database
            // connection instead of verifying only fetching connection string once.
            environmentMock.Verify( e => e.DatabaseConnection, Times.Once);
        }

        [Fact]
        public void UpsertTweetAsync_IgnoresDuplicateTweets()
        {
            // Arrange
            var environmentMock = new Mock<IApiEnvironment>();
            var loggerStub = new NullLogger<DataService>();
            var fakeConfig = FakeConfigurationProvider.BuildDefaultConfiguration();
            IStreamedTweet tweet = new StreamedTweet("id", "text", null, null);

            environmentMock.SetupGet(p => p.DatabaseConnection)
                .Returns(FakeConfigurationProvider.ValidKey);

            var dut = new DataService(
                loggerStub,
                fakeConfig,
                environmentMock.Object);

            dut.UpsertTweetAsync(tweet).Wait();

            // Act
            dut.UpsertTweetAsync(tweet).Wait();

            var count = dut.GetTweetProcessedCountAsync().Result;

            // Assert
            Assert.Equal(1, count);
        }
    }
}
