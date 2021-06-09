namespace Streaming.Api.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Implementation.Data;
    using Xunit;

    public class DataServiceTests
    {
        
        [Fact]
        public void ConnectAsync_WithInvalidConnectionSettingKey_ThrowsException()
        {
            // Arrange
            var environmentMock = new Mock<IApiEnvironment>();
            var loggerStub = new Mock<ILogger>();
            var configStub = new Mock<IConfiguration>();

            var dut = new DataService(
                loggerStub.Object,
                configStub.Object,
                environmentMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await dut.ConnectAsync());
        }

        [Fact]
        public void ConnectAsync_NullOrEmptyConnection_ThrowsException()
        {
            // Arrange
            var environmentMock = new Mock<IApiEnvironment>();
            var loggerStub = new Mock<ILogger>();
            var configStub = new Mock<IConfiguration>();

            environmentMock.SetupGet(p => p.DatabaseConnection)
                .Returns(FakeConfigurationProvider.ValidDbConnectionSettingsKey);

            var dut = new DataService(
                loggerStub.Object,
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
            var loggerStub = new Mock<ILogger>();
            var fakeConfig = FakeConfigurationProvider.BuildDefaultConfiguration();

            environmentMock.SetupGet(p => p.DatabaseConnection)
                .Returns(FakeConfigurationProvider.ValidDbConnectionSettingsKey);

            var dut = new DataService(
                loggerStub.Object,
                fakeConfig,
                environmentMock.Object);

            // Act
            dut.ConnectAsync().Wait();

            // Assert

            // For the purposes of this app, if we get to here, then we've successfully
            // connected to the db.
        }
    }

    public class FakeConfigurationProvider
    {
        public const string ValidDbConnectionSettingsKey = "TestDbConnectionSettingsKey";
        public const string ValidDbConnectionString = "TestDbConnectionString";

        public static IConfiguration BuildDefaultConfiguration()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"TopLevelKey", "TopLevelValue"},
                {"SectionName:SomeKey", "SectionValue"},
                {ValidDbConnectionSettingsKey, ValidDbConnectionString},
                //...populate as needed for tests
            };

            return BuildConfiguration(inMemorySettings);
        }

        public static IConfiguration BuildConfiguration(Dictionary<string, string> config)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            return configuration;
        }
    }
}
