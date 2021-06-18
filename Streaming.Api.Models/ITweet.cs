namespace Streaming.Api.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IStreamedTweet
    {
        /// <summary>
        /// The Tweet's unique identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The tweet's raw text.
        /// </summary>
        string RawTweetText { get; }

        /// <summary>
        /// Hashtags embedded in the tweet.
        /// </summary>
        public IEnumerable<string> HashTags { get; }

        /// <summary>
        /// Uris embedded in the tweet.
        /// </summary>
        IEnumerable<Uri> Uris { get; }
    }
}
