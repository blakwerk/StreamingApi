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