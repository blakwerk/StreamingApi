namespace Streaming.Api.Core.Configuration
{
    using RestSharp;

    /// <summary>
    /// Provides <see cref="IRestClient"/> instances.
    /// </summary>
    public interface IClientFactory
    {
        /// <summary>
        /// Gets an instance of <see cref="IRestClient"/>
        /// </summary>
        IRestClient GetApiV2Client();

        /// <summary>
        /// Gets an instance of <see cref="IRestClient"/>
        /// </summary>
        IRestClient GetApiV1Client();
    }
}
