namespace SampledStreamClient
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Streaming.Api.Core.Services;

    internal class Worker : IHostedService
    {
        private readonly ITweetStreamConnection _streamConnection;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Worker> _logger;

        public Worker(
            ITweetStreamConnection streamConnection,
            IConfiguration configuration,
            ILogger<Worker> logger)
        {
            _streamConnection = streamConnection ?? throw new ArgumentNullException(nameof(streamConnection));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //Log.Logger.Information("Worker starting.");

            _logger.LogInformation("Worker starting.");

            await _streamConnection.ConnectToSampledStreamAsync();

            _logger.LogInformation("Worker finished.");
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // let's crash when shutting down, just for fun.
            throw new NotImplementedException();
        }
    }
}
