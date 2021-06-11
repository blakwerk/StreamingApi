namespace Streaming.Api.Implementation.Data
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Core.Data;
    using System.Threading.Tasks;

    internal class DataService : IDataService
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;
        private readonly IApiEnvironment _apiEnvironment;

        public DataService(
            ILogger<DataService> log, 
            IConfiguration config,
            IApiEnvironment apiEnvironment)
        {
            _log = log;
            _config = config;
            _apiEnvironment = apiEnvironment;
        }
        
        /// <inheritdoc />
        public Task ConnectAsync()
        {
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

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, $"Unable to connect to datastore: {settingKey}");
                throw;
            }
        }
    }
}
