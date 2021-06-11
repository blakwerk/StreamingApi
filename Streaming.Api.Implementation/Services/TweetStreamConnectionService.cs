namespace Streaming.Api.Implementation.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using RestSharp.Authenticators;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Implementation.Configuration;

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
