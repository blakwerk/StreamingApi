namespace Streaming.Api.Core.Configuration
{
    /// <summary>
    /// Specifies configuration keys for the api environment.
    /// </summary>
    public interface IApiEnvironment
    {
        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        string DatabaseConnection { get; }
    }
}