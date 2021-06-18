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
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _tweetQueue = new ConcurrentQueue<IStreamedTweet>();
        }

        /// <inheritdoc />
        public void EnqueueTweetForProcessing(IStreamedTweet tweet)
        {
            _tweetQueue.Enqueue(tweet);
        }

        /// <inheritdoc />
        public async Task ProcessAllEnqueuedTweetsAsync()
        {
            while (!_tweetQueue.IsEmpty)
            {
                var gotTweet = _tweetQueue.TryDequeue(out var tweet);

                if (!gotTweet)
                {
                    continue;
                }

                await _dataService.UpsertTweetAsync(tweet);
            }
        }

        /// <inheritdoc />
        public Task ProcessTweetAsync(IStreamedTweet tweet)
        {
            _logger.LogDebug($"Logging tweet: {tweet.RawTweetText}");
            
            return _dataService.UpsertTweetAsync(tweet);
        }
    }
}
