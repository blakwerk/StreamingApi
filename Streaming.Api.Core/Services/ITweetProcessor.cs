namespace Streaming.Api.Core.Services
{
    using System.Threading.Tasks;
    using Streaming.Api.Models;

    public interface ITweetProcessor
    {
        /// <summary>
        /// Processes tweets.
        /// </summary>
        Task ProcessTweetAsync(IStreamedTweet tweet);

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
