namespace Streaming.Api.Implementation.Data
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Core.Data;
    using System.Threading.Tasks;
    using Streaming.Api.Models;

    internal class DataService : IDataService
    {
        private bool _isConnected;

        private readonly ILogger _log;
        private readonly IConfiguration _config;
        private readonly IApiEnvironment _apiEnvironment;

        private readonly DateTime _processingStart;

        private readonly ConcurrentDictionary<string, int> _processedDomains;
        private readonly ConcurrentDictionary<string, int> _processedHashtags;
        private readonly ConcurrentDictionary<string, int> _processedEmojis;

        private readonly ConcurrentDictionary<string, IStreamedTweet> _processedTweetsRepository;

        public DataService(
            ILogger<DataService> log, 
            IConfiguration config,
            IApiEnvironment apiEnvironment)
        {
            _processingStart = DateTime.UtcNow;

            _log = log ?? throw new ArgumentNullException(nameof(log));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _apiEnvironment = apiEnvironment ?? throw new ArgumentNullException(nameof(apiEnvironment));

            _processedTweetsRepository = new ConcurrentDictionary<string, IStreamedTweet>();
            _processedDomains = new ConcurrentDictionary<string, int>();
            _processedHashtags = new ConcurrentDictionary<string, int>();
            _processedEmojis = new ConcurrentDictionary<string, int>();
        }
        
        /// <inheritdoc />
        public Task ConnectAsync()
        {
            if (_isConnected)
            {
                _log.LogDebug("Datastore already connected.");
                return Task.CompletedTask;
            }

            var settingKey = _apiEnvironment.DatabaseConnection;

            try
            {
                if (string.IsNullOrWhiteSpace(settingKey))
                {
                    throw new ArgumentException("Database connection key is invalid!");
                }

                var connectionString = _config.GetValue<string>(settingKey);

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new ArgumentException("Database connection string is invalid!");
                }

                _log.LogInformation($"Connecting to data store {connectionString}");

                // This is where one would actually connect to the database if it were
                // a real data store. Possibly directly, possibly via a connection
                // multiplexer.

                _log.LogInformation($"Connected to data store {connectionString}");

                _isConnected = true;

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, $"Unable to connect to datastore: {settingKey}");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpsertTweetAsync(IStreamedTweet tweet)
        {
            await this.ConnectAsync().ConfigureAwait(false);

            if (_processedTweetsRepository.ContainsKey(tweet.Id))
            {
                // for the purposes of this data exercise, do not double-process tweets
                return;
            }

            _processedTweetsRepository.AddOrUpdate(tweet.Id, tweet, (_, _) => tweet);
            
            this.UpsertDomains(tweet.Uris);
            this.UpsertHashtags(tweet.HashTags);
            this.UpsertEmojis(tweet.Emojis);

            return;
        }

        /// <inheritdoc />
        public async Task<int> GetTweetProcessedCountAsync()
        {
            await this.ConnectAsync().ConfigureAwait(false);

            return this._processedTweetsRepository.Count;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetTopDomainsAsync(int takeCount)
        {
            await this.ConnectAsync().ConfigureAwait(false);

            var topDomainsKvpSnapshot = _processedDomains.ToList();

            topDomainsKvpSnapshot.Sort((kvp1, kvp2) => kvp1.Value.CompareTo(kvp2.Value));

            var topDomains = topDomainsKvpSnapshot.Select(kvp => kvp.Key).Take(takeCount);

            return topDomains;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetTopHashtagsAsync(int takeCount)
        {
            await this.ConnectAsync().ConfigureAwait(false);

            var topHashtagsKvpSnapshot = _processedHashtags.ToList();

            topHashtagsKvpSnapshot.Sort((kvp1, kvp2) => kvp1.Value.CompareTo(kvp2.Value));

            var topHashtags = topHashtagsKvpSnapshot.Select(kvp => kvp.Key).Take(takeCount);

            return topHashtags;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetTopEmojisAsync(int takeCount)
        {
            await this.ConnectAsync().ConfigureAwait(false);

            var topEmojisKvpSnapshot = _processedEmojis.ToList();

            topEmojisKvpSnapshot.Sort((kvp1, kvp2) => kvp1.Value.CompareTo(kvp2.Value));

            var topEmojis = topEmojisKvpSnapshot.Select(kvp => kvp.Key).Take(takeCount);

            return topEmojis;
        }

        /// <inheritdoc />
        public async Task<int> GetTweetsContainingUrlCountAsync()
        {
            await this.ConnectAsync().ConfigureAwait(false);

            return this._processedTweetsRepository.Values.Count(t => t.ContainsUrl);
        }

        /// <inheritdoc />
        public async Task<int> GetTweetsContainingPhotoUrlCountAsync()
        {
            await this.ConnectAsync().ConfigureAwait(false);

            return this._processedTweetsRepository.Values.Count(t => t.ContainsPhotoUrl);
        }

        /// <inheritdoc />
        public async Task<int> GetTweetsContainingEmojiCountAsync()
        {
            await this.ConnectAsync().ConfigureAwait(false);

            return this._processedTweetsRepository.Values.Count(t => t.ContainsEmoji);
        }

        /// <inheritdoc />
        public async Task<TimeSpan> GetElapsedProcessingTimeAsync()
        {
            await this.ConnectAsync().ConfigureAwait(false);
            
            // this could be pulled from the data store if it were actually remote,
            // but this will satisfice for these purposes.
            var now = DateTime.UtcNow;

            return now - this._processingStart;
        }

        private void UpsertDomains(IEnumerable<Uri> uris)
        {
            if (uris == null)
            {
                return;
            }

            foreach (var uri in uris)
            {
                _processedDomains.AddOrUpdate(uri.Host, 1, (_, i) => i+1);
            }
        }

        private void UpsertHashtags(IEnumerable<string> hashtags)
        {
            if (hashtags == null)
            {
                return;
            }

            foreach (var hashtag in hashtags)
            {
                _processedHashtags.AddOrUpdate(hashtag, 1, (_, i) => i + 1);
            }
        }

        private void UpsertEmojis(IEnumerable<string> emojis)
        {
            if (emojis == null)
            {
                return;
            }

            foreach (var emoji in emojis)
            {
                _processedEmojis.AddOrUpdate(emoji, 1, (_, i) => i + 1);
            }
        }
    }
}
