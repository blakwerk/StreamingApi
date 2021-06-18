namespace Streaming.Api.Core.Services
{
    using System.Threading.Tasks;
    using Streaming.Api.Models;

    /// <summary>
    /// Specifies a tweet processor.
    /// </summary>
    public interface ITweetProcessor
    {
        /// <summary>
        /// Processes tweets.
        /// </summary>
        Task ProcessAllEnqueuedTweetsAsync();

        /// <summary>
        /// Enqueues tweets for processing.
        /// </summary>
        void EnqueueTweetForProcessing(IStreamedTweet tweet);
    }
}
