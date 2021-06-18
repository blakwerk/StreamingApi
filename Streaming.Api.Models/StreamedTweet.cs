namespace Streaming.Api.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class StreamedTweet : IStreamedTweet
    {
        private Lazy<IEnumerable<string>> emojis;

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string RawTweetText { get; }

        /// <inheritdoc />
        public bool ContainsUrl => this.Uris.Any();

        /// <inheritdoc />
        public bool ContainsPhotoUrl {
            get
            {
                return this.Uris.Any(u => u.Host.Contains("pic.twitter.com") || u.Host.Contains("instagram"));
            }
        }

        /// <inheritdoc />
        public bool ContainsEmoji => this.Emojis.Any();

        /// <inheritdoc />
        public IEnumerable<string> HashTags { get; }

        /// <inheritdoc />
        public IEnumerable<string> Emojis => this.emojis.Value;

        /// <inheritdoc />
        public IEnumerable<Uri> Uris { get; }

        public StreamedTweet(
            string id,
            string tweetText, 
            IEnumerable<string> hashTags,
            IEnumerable<string> urls)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"{nameof(id)} cannot be empty!");
            }

            if (string.IsNullOrWhiteSpace(tweetText))
            {
                throw new ArgumentException($"{nameof(tweetText)} cannot be empty!");
            }

            this.Id = id;
            this.RawTweetText = tweetText;

            this.HashTags = hashTags ?? new string [0];

            this.Uris = urls == null ? 
                new Uri[0] : 
                urls.Select(u => new Uri(u));

            this.emojis = new Lazy<IEnumerable<string>>(() => this.ProcessEmoji(tweetText));
        }

        private IEnumerable<string> ProcessEmoji(string input)
        {
            var matches = Regex.Matches(input, EmojiUtils.EmojiRegex);

            var emojiList = new List<string>();

            foreach (var match in matches)
            {
                if (string.IsNullOrWhiteSpace(match?.ToString()))
                {
                    continue;
                }

                emojiList.Add(match.ToString());
            }

            return emojiList;
        }
    }

    /*
     * sample tweet:
{
    "data": [
        {
            "id": "1212092628029698048",
            "text": "We believe the best future version of our API will come from building it with YOU. Here’s to another great year with everyone who builds on the Twitter platform. We can’t wait to continue working with you in the new year. https://t.co/yvxdK6aOo2",
            "possibly_sensitive": false,
            "referenced_tweets": [
                {
                    "type": "replied_to",
                    "id": "1212092627178287104"
                }
            ],
            "entities": {
                "urls": [
                    {
                        "start": 222,
                        "end": 245,
                        "url": "https://t.co/yvxdK6aOo2",
                        "expanded_url": "https://twitter.com/LovesNandos/status/1211797914437259264/photo/1",
                        "display_url": "pic.twitter.com/yvxdK6aOo2"
                    }
                ],
                "annotations": [
                    {
                        "start": 144,
                        "end": 150,
                        "probability": 0.626,
                        "type": "Product",
                        "normalized_text": "Twitter"
                    }
                ]
            },
            "author_id": "2244994945",
            "public_metrics": {
                "retweet_count": 8,
                "reply_count": 2,
                "like_count": 40,
                "quote_count": 1
            },
            "lang": "en",
            "created_at": "2019-12-31T19:26:16.000Z",
            "source": "Twitter Web App",
            "in_reply_to_user_id": "2244994945",
            "attachments": {
                "media_keys": [
                    "16_1211797899316740096"
                ]
            },
            "context_annotations": [
                {
                    "domain": {
                        "id": "119",
                        "name": "Holiday",
                        "description": "Holidays like Christmas or Halloween"
                    },
                    "entity": {
                        "id": "1186637514896920576",
                        "name": " New Years Eve"
                    }
                },
                {
                    "domain": {
                        "id": "119",
                        "name": "Holiday",
                        "description": "Holidays like Christmas or Halloween"
                    },
                    "entity": {
                        "id": "1206982436287963136",
                        "name": "Happy New Year: It’s finally 2020 everywhere!",
                        "description": "Catch fireworks and other celebrations as people across the globe enter the new year.\nPhoto via @GettyImages "
                    }
                },
                {
                    "domain": {
                        "id": "46",
                        "name": "Brand Category",
                        "description": "Categories within Brand Verticals that narrow down the scope of Brands"
                    },
                    "entity": {
                        "id": "781974596752842752",
                        "name": "Services"
                    }
                },
                {
                    "domain": {
                        "id": "47",
                        "name": "Brand",
                        "description": "Brands and Companies"
                    },
                    "entity": {
                        "id": "10045225402",
                        "name": "Twitter"
                    }
                },
                {
                    "domain": {
                        "id": "119",
                        "name": "Holiday",
                        "description": "Holidays like Christmas or Halloween"
                    },
                    "entity": {
                        "id": "1206982436287963136",
                        "name": "Happy New Year: It’s finally 2020 everywhere!",
                        "description": "Catch fireworks and other celebrations as people across the globe enter the new year.\nPhoto via @GettyImages "
                    }
                }
            ]
        }
    ],
    "includes": {
        "tweets": [
            {
                "possibly_sensitive": false,
                "referenced_tweets": [
                    {
                        "type": "replied_to",
                        "id": "1212092626247110657"
                    }
                ],
                "text": "These launches would not be possible without the feedback you provided along the way, so THANK YOU to everyone who has contributed your time and ideas. Have more feedback? Let us know ⬇️ https://t.co/Vxp4UKnuJ9",
                "entities": {
                    "urls": [
                        {
                            "start": 187,
                            "end": 210,
                            "url": "https://t.co/Vxp4UKnuJ9",
                            "expanded_url": "https://twitterdevfeedback.uservoice.com/forums/921790-twitter-developer-labs",
                            "display_url": "twitterdevfeedback.uservoice.com/forums/921790-…",
                            "images": [
                                {
                                    "url": "https://pbs.twimg.com/news_img/1261301555787108354/9yR4UVsa?format=png&name=orig",
                                    "width": 100,
                                    "height": 100
                                },
                                {
                                    "url": "https://pbs.twimg.com/news_img/1261301555787108354/9yR4UVsa?format=png&name=150x150",
                                    "width": 100,
                                    "height": 100
                                }
                            ],
                            "status": 200,
                            "title": "Twitter Developer Feedback",
                            "description": "Share your feedback for the Twitter developer platform",
                            "unwound_url": "https://twitterdevfeedback.uservoice.com/forums/921790-twitter-developer-labs"
                        }
                    ]
                },
                "author_id": "2244994945",
                "public_metrics": {
                    "retweet_count": 3,
                    "reply_count": 1,
                    "like_count": 17,
                    "quote_count": 0
                },
                "lang": "en",
                "created_at": "2019-12-31T19:26:16.000Z",
                "source": "Twitter Web App",
                "in_reply_to_user_id": "2244994945",
                "id": "1212092627178287104"
            }
        ]
    }
}
     */
}
