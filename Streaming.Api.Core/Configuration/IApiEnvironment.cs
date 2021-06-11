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
        /// Gets the base uri configuration key.
        /// </summary>
        string V2ApiBaseUri { get; }
    }
}