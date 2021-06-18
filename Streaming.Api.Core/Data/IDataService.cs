namespace Streaming.Api.Core.Data
{
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
    }
}
