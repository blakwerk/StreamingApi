namespace Streaming.Api.Implementation.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Models;
    using Tweetinvi;
    using Tweetinvi.Events;
    using Tweetinvi.Models.V2;

    internal class TweetStreamConnectionService : ITweetStreamConnection
    {
        private readonly ILogger<TweetStreamConnectionService> _logger;
        private readonly ITweetProcessor _tweetProcessor;
        private readonly IConfiguration _configuration;
        private readonly IApiEnvironment _apiEnvironment;
        private readonly int _sampledTweetsStopCount = -1;

        public TweetStreamConnectionService(
            ILogger<TweetStreamConnectionService> logger,
            ITweetProcessor tweetProcessor,
            IConfiguration configuration,
            IApiEnvironment apiEnvironment)
        {
            _tweetProcessor = tweetProcessor ?? throw new ArgumentNullException(nameof(tweetProcessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiEnvironment = apiEnvironment ?? throw new ArgumentNullException(nameof(apiEnvironment));
        }

        /// <summary>
        /// Connects to twitter sampled stream and sends tweets for processing.
        /// </summary>
        public async Task ConnectToSampledStreamAsync()
        {
            _logger.LogInformation("Beginning stream.");

            try
            {
                var tokenSetting = _apiEnvironment.TwitterAppTokenKey;
                var consumerKeySetting = _apiEnvironment.TwitterAppConsumerKey;
                var consumerSecretSetting = _apiEnvironment.TwitterAppSecretKey;

                if (string.IsNullOrWhiteSpace(tokenSetting) ||
                    string.IsNullOrWhiteSpace(consumerKeySetting) ||
                    string.IsNullOrWhiteSpace(consumerSecretSetting))
                {
                    throw new Exception("Unable to find one or more configuration key settings.");
                }

                var bearerToken = _configuration.GetValue<string>(tokenSetting);
                var consumerKey = _configuration.GetValue<string>(consumerKeySetting);
                var consumerSecretKey = _configuration.GetValue<string>(consumerSecretSetting);

                _logger.LogInformation("Creating stream client.");

                // since the connection to the stream is not the focal point of this, and
                // only serves to simulate high-volume API traffic, it's not made
                // particularly robust. otherwise, better to generate a client from 
                // a factory (for testing), and implement connect-on-disconnect error handling,
                // and either encrypt the twitter api connection settings, or provide them
                // via azure (azure app config, azure key vault, etc).
                var client = new TwitterClient(consumerKey, consumerSecretKey, bearerToken);
                client.Config.TweetMode = TweetMode.Extended;

                var currentSampleCount = 0;

                _logger.LogInformation("Connecting to stream.");
                var sampleStreamV2 = client.StreamsV2.CreateSampleStream();

                sampleStreamV2.EventReceived += SampleStreamV2_EventReceived;

                sampleStreamV2.TweetReceived += (_, args) =>
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(args?.Tweet?.Text))
                        {
                            _logger.LogWarning("Empty tweet received.");
                            // In the case of an error service, alerts, or circuit breaker,
                            // this is where we would want to send a report.
                            
                            return;
                        }

                        _logger.LogDebug(args.Tweet.Text);

                        var streamedTweet = BuildStreamedTweet(args.Tweet);
                        
                        _tweetProcessor.EnqueueTweetForProcessing(streamedTweet);

                        // threshold disabled. process indefinitely.
                        if (_sampledTweetsStopCount < 0)
                        {
                            return;
                        }

                        currentSampleCount++;
                        if (currentSampleCount >= _sampledTweetsStopCount)
                        {
                            sampleStreamV2.StopStream();
                            _logger.LogInformation("Completing stream.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An exception occurred.");
                        // for the purposes of this stream reader, skip forward if we're hitting exceptions 
                        currentSampleCount += 10;
                    }
                };
                
                await sampleStreamV2.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown when connecting to stream");
                throw;
            }
        }

        private void SampleStreamV2_EventReceived(object sender, StreamEventReceivedArgs e)
        {
            _logger.LogDebug($"Twitter stream event received: {e.Json}");

            // if we were going to have a reconnect service, check the event here and reconnect.
        }

        private static IStreamedTweet BuildStreamedTweet(TweetV2 tweet)
        {
            var hashtags = tweet.Entities?.Hashtags?.Select(h => h.Tag);
            var urls = tweet.Entities?.Urls?.Select(u => u.ExpandedUrl);
            
            return new StreamedTweet(tweet.Id, tweet.Text, hashtags, urls);
        }
    }
}
