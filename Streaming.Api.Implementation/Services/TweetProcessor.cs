namespace Streaming.Api.Implementation.Services
{
    using Streaming.Api.Core.Data;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Models;
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    internal class TweetProcessor : ITweetProcessor
    {
        private readonly ILogger<TweetProcessor> _logger;
        private readonly IDataService _dataService;

        private readonly ConcurrentQueue<IStreamedTweet> _tweetQueue;

        public TweetProcessor(
            ILogger<TweetProcessor> logger,
            IDataService dataService)
        {
            this._dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._tweetQueue = new ConcurrentQueue<IStreamedTweet>();
        }

        /// <inheritdoc />
        public void EnqueueTweetForProcessing(IStreamedTweet tweet)
        {
            this._tweetQueue.Enqueue(tweet);
        }

        /// <inheritdoc />
        public async Task ProcessAllEnqueuedTweetsAsync()
        {
            while (!this._tweetQueue.IsEmpty)
            {
                var gotTweet = this._tweetQueue.TryDequeue(out var tweet);

                if (!gotTweet)
                {
                    continue;
                }

                await this._dataService.UpsertTweetAsync(tweet);
            }
        }

        /// <inheritdoc />
        public Task ProcessTweetAsync(IStreamedTweet tweet)
        {
            this._logger.LogDebug($"Logging tweet: {tweet.RawTweetText}");
            
            return this._dataService.UpsertTweetAsync(tweet);
        }
    }
}
