namespace Streaming.Api.Implementation.Services
{
    using Microsoft.Extensions.Logging;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Models;

    using System;
    using System.Text;
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
            this._logger.LogInformation($"Processed tweets: {statsReport.TotalProcessedTweetCount}\n" +
                                        $"\t % tweets with url: {statsReport.PercentTweetsContainingUrl:P} (count: {statsReport.UrlContainingTweetCount})\n" +
                                        $"\t % tweets with photo url: {statsReport.PercentTweetsContainingPhotoUrl:P} (count: {statsReport.PhotoUrlContainingTweetCount})\n" +
                                        $"\t % tweets with emoji: {statsReport.PercentTweetsContainingEmoji:P} (count: {statsReport.EmojiContainingTweetCount})\n" +
                                        $"\t {this.BuildTopDomainsString(statsReport)}\n" +
                                        $"\t {this.BuildTopHashtagsString(statsReport)}\n" +
                                        $"\t {this.BuildTopEmojisString(statsReport)}\n");
        }

        private string BuildTopDomainsString(TweetStatsReport report)
        {
            var sb = new StringBuilder("Top domains:");
            foreach (var domain in report.TopTenUrlDomains)
            {
                sb.Append($" {domain},");
            }

            sb.Remove(sb.Length-1, 1);

            return sb.ToString();
        }

        private string BuildTopHashtagsString(TweetStatsReport report)
        {
            var sb = new StringBuilder("Top hashtags:");
            foreach (var hashtag in report.TopTenHashtags)
            {
                sb.Append($" {hashtag},");
            }

            sb.Remove(sb.Length-1, 1);

            return sb.ToString();
        }

        private string BuildTopEmojisString(TweetStatsReport report)
        {
            var sb = new StringBuilder("Top emojis:");
            foreach (var emoji in report.TopTenEmoji)
            {
                sb.Append($" {emoji},");
            }

            sb.Remove(sb.Length-1, 1);

            return sb.ToString();
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

            var uriCount = await this._dataService.GetTweetsContainingUrlCountAsync().ConfigureAwait(false);

            var photoUriCount = await this._dataService.GetTweetsContainingPhotoUrlCountAsync().ConfigureAwait(false);

            var emojiCount = await this._dataService.GetTweetsContainingEmojiCountAsync().ConfigureAwait(false);

            var topTenHashtags = await this._dataService.GetTopHashtagsAsync(10);
            var topTenEmoji = await this._dataService.GetTopEmojisAsync(10);
            var topTenDomains = await this._dataService.GetTopDomainsAsync(10);

            return new TweetStatsReport
            {
                TotalProcessedTweetCount = tweetCount,
                UrlContainingTweetCount = uriCount,
                PhotoUrlContainingTweetCount = photoUriCount,
                PercentTweetsContainingUrl = (uriCount / (double) tweetCount),
                PercentTweetsContainingPhotoUrl = (photoUriCount / (double) tweetCount),
                PercentTweetsContainingEmoji = (emojiCount / (double) tweetCount),
                TopTenHashtags = topTenHashtags,
                TopTenEmoji = topTenEmoji,
                TopTenUrlDomains = topTenDomains,
            };
        }
    }
}
