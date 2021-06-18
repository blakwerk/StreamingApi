namespace Streaming.Api.Core.Configuration
{
    /// <summary>
    /// Specifies configuration keys for the api environment.
    /// </summary>
    public interface IApiEnvironment
    {
        /// <summary>
        /// Gets the database connection string key.
        /// </summary>
        string DatabaseConnection { get; }

        /// <summary>
        /// Gets the configuration key for the twitter api consumer key.
        /// </summary>
        string TwitterAppConsumerKey { get; }

        /// <summary>
        /// Gets the configuration key for the twitter api secret key.
        /// </summary>
        string TwitterAppSecretKey { get; }

        /// <summary>
        /// Gets the configuration key for the twitter api token.
        /// </summary>
        string TwitterAppTokenKey { get; }
    }
}