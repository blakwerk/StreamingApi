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
        /// A flag indicating that the tweet contains a url.
        /// </summary>
        bool ContainsUrl { get; }

        /// <summary>
        /// A flag indicating that the tweet contains a photo url.
        /// </summary>
        bool ContainsPhotoUrl { get; }

        /// <summary>
        /// A flag indicating that the tweet contains an emoji.
        /// </summary>
        bool ContainsEmoji { get; }

        /// <summary>
        /// Hashtags embedded in the tweet.
        /// </summary>
        IEnumerable<string> HashTags { get; }

        /// <summary>
        /// Emojis embedded in the tweet.
        /// </summary>
        IEnumerable<string> Emojis { get; }

        /// <summary>
        /// Uris embedded in the tweet.
        /// </summary>
        IEnumerable<Uri> Uris { get; }
    }
}
