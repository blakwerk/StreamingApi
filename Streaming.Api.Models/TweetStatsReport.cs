namespace Streaming.Api.Models
{
    using System;
    using System.Collections.Generic;

    public class TweetStatsReport
    {
        // Here's what we need to know:
        // 1: The number of tweets received
        // 2: Avg tweets per hour/min/sec
        // 3: Does it contain Emojis - if so, which ones (need all)
        // 4: Does it contain hashtags? If so, which ones (need all)
        // 5: Does it contain a url? If so, track it (& its domain)
        // 6: Does it contain a photo url (twitter or insta)? If so, track it

        /// <summary>
        /// Gets or sets the total elapsed processing time.
        /// </summary>
        public TimeSpan ElapsedProcessingTime { get; set; }

        /// <summary>
        /// Gets or sets the total count of tweets processed.
        /// </summary>
        public int TotalProcessedTweetCount { get; set; }

        /// <summary>
        /// Gets or sets the count of tweets with links.
        /// </summary>
        public int UrlContainingTweetCount { get; set; }

        /// <summary>
        /// Gets or sets the count of tweets containing links to images, including
        /// image links.
        /// </summary>
        public int PhotoUrlContainingTweetCount { get; set; }

        /// <summary>
        /// Gets or sets the count of tweets containing any emoji.
        /// </summary>
        public int EmojiContainingTweetCount { get; set; }

        /// <summary>
        /// Gets or sets the percent of tweets containing a link, including image
        /// links.
        /// </summary>
        public double PercentTweetsContainingUrl { get; set; }

        /// <summary>
        /// Gets or sets the percent of tweets containing an image link.
        /// </summary>
        public double PercentTweetsContainingPhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the percent of tweets containing emoji.
        /// </summary>
        public double PercentTweetsContainingEmoji { get; set; }

        /// <summary>
        /// Gets or sets the top ten hashtags used in tweets.
        /// </summary>
        public IEnumerable<string> TopTenHashtags { get; set; }

        /// <summary>
        /// Gets or sets the top ten emoji used in tweets.
        /// </summary>
        public IEnumerable<string> TopTenEmoji { get; set; }

        /// <summary>
        /// Gets the top ten domains seen in links in tweets.
        /// </summary>
        public IEnumerable<string> TopTenUrlDomains { get; set; }
    }
}