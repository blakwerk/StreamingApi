namespace SampledStreamClient
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Streaming.Api.Core.Services;

    internal class Worker : IHostedService
    {
        private readonly ITweetStreamConnection _streamConnection;
        private readonly IConsoleUpdater _consoleUpdater;
        private readonly ITweetProcessor _tweetProcessor;

        private readonly IConfiguration _configuration;
        private readonly ILogger<Worker> _logger;
        
        public Worker(
            ITweetStreamConnection streamConnection,
            IConsoleUpdater consoleUpdater,
            ITweetProcessor tweetProcessor,
            IConfiguration configuration,
            ILogger<Worker> logger)
        {
            _streamConnection = streamConnection ?? throw new ArgumentNullException(nameof(streamConnection));
            _consoleUpdater = consoleUpdater ?? throw new ArgumentNullException(nameof(consoleUpdater));
            _tweetProcessor = tweetProcessor ?? throw new ArgumentNullException(nameof(tweetProcessor));

            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker starting.");

            var streamTask = _streamConnection.ConnectToSampledStreamAsync();
            var updateTask = _consoleUpdater.StartUpdatesAsync(TimeSpan.FromSeconds(2));
            var processorTask = this.ProcessTweetsAsync();

            await Task.WhenAll(streamTask, updateTask, processorTask).ConfigureAwait(false);
            
            _logger.LogInformation("Worker finished.");
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Working stopping.");
            return Task.CompletedTask;
        }

        private async Task ProcessTweetsAsync()
        {
            while (true)
            {
                await _tweetProcessor.ProcessAllEnqueuedTweetsAsync().ConfigureAwait(false);
            }
        }
    }
}
