namespace Streaming.Api.Tests.Services
{
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Streaming.Api.Core.Data;
    using Streaming.Api.Implementation.Services;
    using Streaming.Api.Models;
    using Xunit;

    public class TweetProcessorTests
    {
        [Fact]
        public void ProcessAllEnqueuedTweetsAsync_InitializesDataServiceConnection()
        {
            // Arrange
            var dataServiceMock = new Mock<IDataService>();
            var loggerStub = new NullLogger<TweetProcessor>();

            var dut = new TweetProcessor(
                loggerStub,
                dataServiceMock.Object);

            // Act
            dut.ProcessAllEnqueuedTweetsAsync().Wait();

            // Assert
            dataServiceMock.Verify( ds => ds.ConnectAsync(), Times.Once);
        }

        [Fact]
        public void ProcessAllEnqueuedTweetsAsync_WhenTweetsExist_ProcessesTweets()
        {
            // Arrange
            var dataServiceMock = new Mock<IDataService>();
            var loggerStub = new NullLogger<TweetProcessor>();
            IStreamedTweet tweet = new StreamedTweet("id", "text", null, null);

            var dut = new TweetProcessor(
                loggerStub,
                dataServiceMock.Object);

            dut.EnqueueTweetForProcessing(tweet);

            // Act
            dut.ProcessAllEnqueuedTweetsAsync().Wait();

            // Assert
            dataServiceMock.Verify( ds => ds.ConnectAsync(), Times.Once);
        }

    }
}
