namespace Streaming.Api.Tests.Configuration
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Implementation.Configuration;
    using Streaming.Api.Tests.Data;
    using Xunit;

    public class RestClientFactoryTests
    {
        [Fact]
        public void GetApiV2Client_WithInvalidConnectionSettingKey_ThrowsArgumentException()
        {
            // Arrange
            var environmentMock = new Mock<IApiEnvironment>();
            var loggerStub = new NullLogger<ClientFactory>();
            var configStub = new Mock<IConfiguration>();

            var dut = new ClientFactory(
                loggerStub,
                configStub.Object,
                environmentMock.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => dut.GetApiV2Client());
        }

        [Fact]
        public void GetApiV2Client_NullOrEmptyUri_ThrowsArgumentException()
        {
            // Arrange
            var environmentMock = new Mock<IApiEnvironment>();
            var loggerStub = new NullLogger<ClientFactory>();
            var configStub = new Mock<IConfiguration>();

            environmentMock.SetupGet(p => p.V2ApiBaseUri)
                .Returns(FakeConfigurationProvider.ValidKey);

            var dut = new ClientFactory(
                loggerStub,
                configStub.Object,
                environmentMock.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => dut.GetApiV2Client());
        }
    }
}
