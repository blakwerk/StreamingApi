namespace Streaming.Api.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Streaming.Api.Models;

    public interface IDataService
    {
        /// <summary>
        /// Connects to the data source
        /// </summary>
        Task ConnectAsync();

        /// <summary>
        /// Insert or updates a tweet.
        /// </summary>
        Task UpsertTweetAsync(IStreamedTweet tweet);

        /// <summary>
        /// Gets a count of tweets processed.
        /// </summary>
        Task<int> GetTweetProcessedCountAsync();

        /// <summary>
        /// Gets a collection of the most-often processed domains in tweets.
        /// </summary>
        Task<IEnumerable<string>> GetTopDomainsAsync(int takeCount);

        /// <summary>
        /// Gets a collection of the most-often processed hashtags in tweets.
        /// </summary>
        Task<IEnumerable<string>> GetTopHashtagsAsync(int takeCount);

        /// <summary>
        /// Gets a collection of the most-often processed emojis in tweets.
        /// </summary>
        Task<IEnumerable<string>> GetTopEmojisAsync(int takeCount);

        /// <summary>
        /// Gets a count of tweets containing any url.
        /// </summary>
        Task<int> GetTweetsContainingUrlCountAsync();

        /// <summary>
        /// Gets a count of tweets containing a url with a link to
        /// instagram or pic.twitter.com
        /// </summary>
        Task<int> GetTweetsContainingPhotoUrlCountAsync();

        /// <summary>
        /// Gets a count of tweets containing emoji
        /// </summary>
        Task<int> GetTweetsContainingEmojiCountAsync();

        /// <summary>
        /// Gets total elapsed processing time since startup.
        /// </summary>
        Task<TimeSpan> GetElapsedProcessingTimeAsync();
    }
}
