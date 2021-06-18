namespace Streaming.Api.Implementation.Services
{
    using Microsoft.Extensions.Logging;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Models;

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Streaming.Api.Core.Data;

    internal class ConsoleUpdater : IConsoleUpdater
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ConsoleUpdater> _logger;
        private const int MaxUpdates = 10000;

        public ConsoleUpdater(
            IReportService reportService, 
            ILogger<ConsoleUpdater> logger)
        {
            this._reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async void StartUpdates(TimeSpan updateInterval)
        {
            this._logger.LogInformation("Beginning update timer.");
            var updateCount = 0;

            while (updateCount < MaxUpdates)
            {
                var delay = Task.Delay(updateInterval);
                
                await this.UpdateAsync().ConfigureAwait(false);
                updateCount++;

                await delay.ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task StartUpdatesAsync(TimeSpan updateInterval)
        {
            this._logger.LogInformation("Beginning update timer.");
            var updateCount = 0;

            while (updateCount < MaxUpdates)
            {
                var delay = Task.Delay(updateInterval);
                
                await this.UpdateAsync().ConfigureAwait(false);
                updateCount++;

                await delay.ConfigureAwait(false);
            }
        }

        private async Task UpdateAsync()
        {
            var statsReport = await this._reportService.QueryStatisticsAsync();

            // report stats:
            this._logger.LogInformation($"Processed tweets: {statsReport.TotalProcessedTweets}");
        }
    }

    internal class ReportService : IReportService
    {
        private readonly IDataService _dataService;

        public ReportService(IDataService dataService)
        {
            this._dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        /// <inheritdoc />
        public async Task<TweetStatsReport> QueryStatisticsAsync()
        {
            var tweetCount = await this._dataService.GetTweetProcessedCountAsync().ConfigureAwait(false);

            return new TweetStatsReport
            {
                TotalProcessedTweets = tweetCount,
            };
        }
    }
}
