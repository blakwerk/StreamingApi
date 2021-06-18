namespace Streaming.Api.Implementation.Services
{
    using System;
    using System.Threading.Tasks;
    using Streaming.Api.Core.Data;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Models;

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
            var tweetCountTask = this._dataService.GetTweetProcessedCountAsync();
            var uriCountTask = this._dataService.GetTweetsContainingUrlCountAsync();
            var photoUriCountTask = this._dataService.GetTweetsContainingPhotoUrlCountAsync();
            var emojiCountTask = this._dataService.GetTweetsContainingEmojiCountAsync();

            var topTenHashtagsTask = this._dataService.GetTopHashtagsAsync(10);
            var topTenEmojiTask = this._dataService.GetTopEmojisAsync(10);
            var topTenDomainsTask = this._dataService.GetTopDomainsAsync(10);

            var elapsedProcessingTask = this._dataService.GetElapsedProcessingTimeAsync();

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