namespace Streaming.Api.Implementation.Services
{
    using Microsoft.Extensions.Logging;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Models;
    using System;
    using System.Text;
    using System.Threading.Tasks;

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
        public async Task StartUpdatesAsync(TimeSpan updateInterval)
        {
            _logger.LogInformation("Beginning update timer.");
            var updateCount = 0;

            while (updateCount < MaxUpdates)
            {
                var delayTask = Task.Delay(updateInterval);
                
                await this.UpdateAsync().ConfigureAwait(false);
                updateCount++;

                await delayTask.ConfigureAwait(false);
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
}
