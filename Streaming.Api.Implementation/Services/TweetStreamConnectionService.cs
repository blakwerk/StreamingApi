namespace Streaming.Api.Implementation.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Models;
    using Tweetinvi;
    using Tweetinvi.Events;
    using Tweetinvi.Models.V2;

    internal class TweetStreamConnectionService : ITweetStreamConnection
    {
        private readonly ILogger<TweetStreamConnectionService> _logger;
        private ITweetProcessor _tweetProcessor;
        private readonly IConfiguration _configuration;

        public TweetStreamConnectionService(
            ILogger<TweetStreamConnectionService> logger,
            ITweetProcessor tweetProcessor,
            IConfiguration configuration)
        {
            _tweetProcessor = tweetProcessor ?? throw new ArgumentNullException(nameof(tweetProcessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task ConnectToSampledStreamAsync()
        {
            _logger.LogInformation("Beginning stream.");

            try
            {
                //TODO grab this from config
                var bearerToken = "AAAAAAAAAAAAAAAAAAAAAN0CQgEAAAAAPtniRfuF%2BBWhLQ6DpQvltGc5En0%3DkWZQgqeNCXRWiPsQhVMdhM7gt8ydq7tQAvZfgCZNTz7UdfHxm6";

                var consumerKey = "PyYoQXHCrJyap2kun19yHAHjO";
                var consumerSecretKey = "jFRZFvS8LyCVbiJTNESLujnKfGJTrjEiDvnt2rb09v0sGAHiNX";

                _logger.LogInformation("Creating stream client.");

                var client = new TwitterClient(consumerKey, consumerSecretKey, bearerToken);
                client.Config.TweetMode = TweetMode.Extended;

                var currentSampleCount = 0;
                var sampleStopCount = 10000;

                _logger.LogInformation("Connecting to stream.");
                var sampleStreamV2 = client.StreamsV2.CreateSampleStream();

                sampleStreamV2.EventReceived += SampleStreamV2_EventReceived;

                sampleStreamV2.TweetReceived += (sender, args) =>
                {
                    try
                    {
                        //_logger.LogDebug(args.Tweet.Text);

                        if (string.IsNullOrWhiteSpace(args?.Tweet?.Text))
                        {
                            _logger.LogWarning("Empty tweet received.");
                            // In the case of an error service, alerts, or circuit breaker,
                            // this is where we would want to send a report.
                            
                            return;
                        }

                        var streamedTweet = BuildStreamedTweet(args.Tweet);
                        
                        _tweetProcessor.EnqueueTweetForProcessing(streamedTweet);

                        // threshold disabled. process indefinitely.
                        if (sampleStopCount < 0)
                        {
                            return;
                        }

                        currentSampleCount++;
                        if (currentSampleCount >= sampleStopCount)
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
            //_logger.LogInformation($"Twitter stream event received: {e.Json}");
        }

        private static IStreamedTweet BuildStreamedTweet(TweetV2 tweet)
        {
            var hashtags = tweet.Entities?.Hashtags?.Select(h => h.Tag);
            var urls = tweet.Entities?.Urls?.Select(u => u.ExpandedUrl);
            
            return new StreamedTweet(tweet.Id, tweet.Text, hashtags, urls);
        }
    }
}
