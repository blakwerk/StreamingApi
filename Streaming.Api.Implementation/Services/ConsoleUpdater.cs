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
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async void StartUpdates(TimeSpan updateInterval)
        {
            _logger.LogInformation("Beginning update timer.");
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
            _logger.LogInformation("Beginning update timer.");
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
            var statsReport = await _reportService.QueryStatisticsAsync();

            // report stats:
            _logger.LogInformation($"Processed tweets: {statsReport.TotalProcessedTweetCount}\n" +
                                        $"\t {this.BuildElapsedTimeString(statsReport)}\n" +
                                        $"\t % tweets with url: {statsReport.PercentTweetsContainingUrl:P} (count: {statsReport.UrlContainingTweetCount})\n" +
                                        $"\t % tweets with photo url: {statsReport.PercentTweetsContainingPhotoUrl:P} (count: {statsReport.PhotoUrlContainingTweetCount})\n" +
                                        $"\t % tweets with emoji: {statsReport.PercentTweetsContainingEmoji:P} (count: {statsReport.EmojiContainingTweetCount})\n" +
                                        $"\t {this.BuildTopDomainsString(statsReport)}\n" +
                                        $"\t {this.BuildTopHashtagsString(statsReport)}\n" +
                                        $"\t {this.BuildTopEmojisString(statsReport)}\n");
        }

        private string BuildElapsedTimeString(TweetStatsReport report)
        {
            var totalTweets = report.TotalProcessedTweetCount;

            var elapsedSeconds = report.ElapsedProcessingTime.TotalSeconds;
            var elapsedMinutes = report.ElapsedProcessingTime.TotalMinutes;
            var elapsedHours = report.ElapsedProcessingTime.TotalHours;

            var tweetsPerHour = totalTweets / elapsedHours;
            var tweetsPerMinute = totalTweets / elapsedMinutes;
            var tweetsPerSecond = totalTweets / elapsedSeconds;

            return $"Tweet stats: tweets/hour: {tweetsPerHour:N1}, " +
                   $"tweets/min: {tweetsPerMinute:N1}, tweets/sec: {tweetsPerSecond:N1}";
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
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        /// <inheritdoc />
        public async Task<TweetStatsReport> QueryStatisticsAsync()
        {
            var tweetCountTask = _dataService.GetTweetProcessedCountAsync();
            var uriCountTask = _dataService.GetTweetsContainingUrlCountAsync();
            var photoUriCountTask = _dataService.GetTweetsContainingPhotoUrlCountAsync();
            var emojiCountTask = _dataService.GetTweetsContainingEmojiCountAsync();

            var topTenHashtagsTask = _dataService.GetTopHashtagsAsync(10);
            var topTenEmojiTask = _dataService.GetTopEmojisAsync(10);
            var topTenDomainsTask = _dataService.GetTopDomainsAsync(10);

            var elapsedProcessingTask = _dataService.GetElapsedProcessingTimeAsync();

            await Task.WhenAll(
                    tweetCountTask,
                    uriCountTask,
                    photoUriCountTask,
                    emojiCountTask,
                    topTenHashtagsTask,
                    topTenDomainsTask,
                    topTenEmojiTask,
                    elapsedProcessingTask)
                .ConfigureAwait(false);

            var tweetCount = tweetCountTask.Result;
            var uriCount = uriCountTask.Result;
            var photoUriCount = photoUriCountTask.Result;
            var emojiCount = emojiCountTask.Result;

            var topTenHashtags = topTenHashtagsTask.Result;
            var topTenDomains = topTenDomainsTask.Result;
            var topTenEmoji = topTenEmojiTask.Result;

            var elapsedProcessingTime = elapsedProcessingTask.Result;

            return new TweetStatsReport
            {
                TotalProcessedTweetCount = tweetCount,
                UrlContainingTweetCount = uriCount,
                PhotoUrlContainingTweetCount = photoUriCount,
                EmojiContainingTweetCount = emojiCount,
                PercentTweetsContainingUrl = (uriCount / (double) tweetCount),
                PercentTweetsContainingPhotoUrl = (photoUriCount / (double) tweetCount),
                PercentTweetsContainingEmoji = (emojiCount / (double) tweetCount),
                TopTenHashtags = topTenHashtags,
                TopTenEmoji = topTenEmoji,
                TopTenUrlDomains = topTenDomains,
                ElapsedProcessingTime = elapsedProcessingTime,
            };
        }
    }
}
