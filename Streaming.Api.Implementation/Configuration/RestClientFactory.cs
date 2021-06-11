namespace Streaming.Api.Implementation.Configuration
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using Streaming.Api.Core.Configuration;

    internal class ClientFactory : IClientFactory
    {
        private readonly ILogger<ClientFactory> _logger;
        private readonly IConfiguration _config;
        private readonly IApiEnvironment _apiEnvironment;

        public ClientFactory(
            ILogger<ClientFactory> logger,
            IConfiguration config,
            IApiEnvironment apiEnvironment)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            this._apiEnvironment = apiEnvironment ?? throw new ArgumentNullException(nameof(apiEnvironment));
        }

        /// <inheritdoc />
        public IRestClient GetApiV2Client()
        {
            var settingKey = _apiEnvironment.V2ApiBaseUri;

            if (string.IsNullOrWhiteSpace(settingKey))
            {
                this._logger.LogError("Setting key for v2 api base returned nothing.");
                throw new ArgumentException("Api endpoint key is invalid!");
            }

            var baseUri = _config.GetValue<string>(settingKey);

            if (string.IsNullOrWhiteSpace(baseUri))
            {
                this._logger.LogError($"Setting key {settingKey} returned nothing.");
                throw new ArgumentException("Api endpoint is invalid!");
            }

            return new RestClient(baseUri);
        }

        /// <inheritdoc />
        public IRestClient GetApiV1Client()
        {
            throw new NotImplementedException("Implement this once we need to use v1.1");
        }
    }
}
