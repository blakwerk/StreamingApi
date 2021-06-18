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
            this._consoleUpdater = consoleUpdater ?? throw new ArgumentNullException(nameof(consoleUpdater));
            this._tweetProcessor = tweetProcessor ?? throw new ArgumentNullException(nameof(tweetProcessor));

            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker starting.");

            // connect to stream:
            var streamTask = _streamConnection.ConnectToSampledStreamAsync();
            var updateTask = this._consoleUpdater.StartUpdatesAsync(TimeSpan.FromSeconds(5));
            var processorTask = this.ProcessTweetsAsync();

            await Task.WhenAll(streamTask, updateTask, processorTask).ConfigureAwait(false);
            
            _logger.LogInformation("Worker finished.");
        }

        private async Task ProcessTweetsAsync()
        {
            while (true)
            {
                await this._tweetProcessor.ProcessAllEnqueuedTweetsAsync();
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Working stopping.");
            return Task.CompletedTask;
        }
    }

    internal class UpdaterWorker : IHostedService
    {
        private readonly ITweetStreamConnection _streamConnection;
        private readonly IConsoleUpdater _consoleUpdater;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UpdaterWorker> _logger;

        public UpdaterWorker(
            ITweetStreamConnection streamConnection,
            IConsoleUpdater consoleUpdater,
            IConfiguration configuration,
            ILogger<UpdaterWorker> logger)
        {
            _streamConnection = streamConnection ?? throw new ArgumentNullException(nameof(streamConnection));
            this._consoleUpdater = consoleUpdater ?? throw new ArgumentNullException(nameof(consoleUpdater));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updater worker starting.");

            // begin console updates:
            await this._consoleUpdater.StartUpdatesAsync(TimeSpan.FromSeconds(5));

            _logger.LogInformation("Updater worker finished.");
        }


        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Working stopping.");
            return Task.CompletedTask;
        }
    }

    internal class TweetProcessorWorker : IHostedService
    {
        private readonly ITweetProcessor _tweetProcessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TweetProcessorWorker> _logger;

        public TweetProcessorWorker(
            ITweetProcessor tweetProcessor,
            IConfiguration configuration,
            ILogger<TweetProcessorWorker> logger)
        {
            this._tweetProcessor = tweetProcessor ?? throw new ArgumentNullException(nameof(tweetProcessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // process enqueued tweets
            while (true)
            {
                await _tweetProcessor.ProcessAllEnqueuedTweetsAsync();
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    internal class StreamReaderWorker : IHostedService
    {
        private readonly ITweetStreamConnection _streamConnection;
        private readonly IConsoleUpdater _consoleUpdater;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StreamReaderWorker> _logger;

        public StreamReaderWorker(
            ITweetStreamConnection streamConnection,
            IConsoleUpdater consoleUpdater,
            IConfiguration configuration,
            ILogger<StreamReaderWorker> logger)
        {
            _streamConnection = streamConnection ?? throw new ArgumentNullException(nameof(streamConnection));
            this._consoleUpdater = consoleUpdater ?? throw new ArgumentNullException(nameof(consoleUpdater));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker starting.");

            // connect to stream:
            await _streamConnection.ConnectToSampledStreamAsync().ConfigureAwait(false);
            
            _logger.LogInformation("Worker finished.");
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Working stopping.");
            return Task.CompletedTask;
        }
    }
}
