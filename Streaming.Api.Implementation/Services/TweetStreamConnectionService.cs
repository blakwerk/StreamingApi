namespace Streaming.Api.Implementation.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using RestSharp.Authenticators;
    using Streaming.Api.Core.Services;
    using Tweetinvi;
    using Tweetinvi.Events;

    internal class TweetStreamConnectionService : ITweetStreamConnection
    {
        private readonly ILogger<TweetStreamConnectionService> _logger;
        private readonly IRestClient _v2Client;
        private readonly IConfiguration _configuration;

        public TweetStreamConnectionService(
            ILogger<TweetStreamConnectionService> logger,
            //IClientFactory clientFactory,
            IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //_v2Client = clientFactory?.GetApiV2Client() ?? throw new ArgumentException("Cannot create streaming client!");
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            this._v2Client = new RestClient("https://api.twitter.com/2");
        }

        public async Task ConnectToSampledStreamAsync()
        {
            _logger.LogInformation("Connecting to stream.");

            try
            {
                var bearerToken = "AAAAAAAAAAAAAAAAAAAAAN0CQgEAAAAAPtniRfuF%2BBWhLQ6DpQvltGc5En0%3DkWZQgqeNCXRWiPsQhVMdhM7gt8ydq7tQAvZfgCZNTz7UdfHxm6";

                var consumerKey = "PyYoQXHCrJyap2kun19yHAHjO";
                var consumerSecretKey = "jFRZFvS8LyCVbiJTNESLujnKfGJTrjEiDvnt2rb09v0sGAHiNX";

                var client = new TwitterClient(consumerKey, consumerSecretKey, bearerToken);
                //var authRequest = await client.Auth.RequestAuthenticationUrlAsync();

                var currentSampleCount = 0;
                var sampleStopCount = 10000;

                this._logger.LogInformation("Beginning stream.");
                var sampleStreamV2 = client.StreamsV2.CreateSampleStream();
                sampleStreamV2.EventReceived += SampleStreamV2_EventReceived;

                sampleStreamV2.TweetReceived += async (sender, args) =>
                {
                    try
                    {
                        this._logger.LogDebug(args.Tweet.Text);
                        
                        //TODO map streaming tweets to "incoming API objects" and send to our API (or queue them)

                        currentSampleCount++;
                        if (currentSampleCount >= sampleStopCount)
                        {
                            sampleStreamV2.StopStream();
                            this._logger.LogInformation("Completing stream.");
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "An exception occurred.");
                        // for the purposes of this stream reader, skip forward if we're hitting exceptions 
                        currentSampleCount += 10;
                    }
                };

                //var parameters = new StartSampleStreamV2Parameters{}

                await sampleStreamV2.StartAsync();

                //this._logger.LogInformation(response.Content);

                //if (response.IsSuccessful)
                //{
                //    _logger.LogInformation("Stream connected.");
                //}
                //else
                //{
                //    _logger.LogError("Unable to connect to stream! " +
                //                     $"Response: {(int)response.StatusCode} {response.StatusCode}");
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown when connecting to stream");
                throw;
            }
        }

        private void SampleStreamV2_EventReceived(object sender, StreamEventReceivedArgs e)
        {
            this._logger.LogInformation($"Twitter stream event received: {e.Json}");
        }

        public async Task ConnectToSampledStreamAsync_RestClient()
        {
            _logger.LogInformation("Connecting to stream.");

            try
            {
                //var client = new RestClient("https://api.twitter.com/1.1")
                //{
                //    Authenticator = new HttpBasicAuthenticator(username: "buyme1coffee", password: @"8M#L&o4JcaVT"),
                //};

                //_v2Client.Authenticator = new HttpBasicAuthenticator(
                //    username: "buyme1coffee", 
                //    password: @"8M#L&o4JcaVT");

                var token = "AAAAAAAAAAAAAAAAAAAAAN0CQgEAAAAAPtniRfuF%2BBWhLQ6DpQvltGc5En0%3DkWZQgqeNCXRWiPsQhVMdhM7gt8ydq7tQAvZfgCZNTz7UdfHxm6";
                this._v2Client.Authenticator = new JwtAuthenticator(token);

                //_v2Client.Timeout = -1;

                var request = new RestRequest("tweets/sample/stream", DataFormat.Json)
                {
                    Method = Method.GET,
                };

                //var response = _client.Get(request);
                var response = await _v2Client.ExecuteAsync(request);

                this._logger.LogInformation(response.Content);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation("Stream connected.");
                }
                else
                {
                    _logger.LogError("Unable to connect to stream! " +
                                          $"Response: {(int)response.StatusCode} {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown when connecting to stream");
                throw;
            }
        }
    }
}
