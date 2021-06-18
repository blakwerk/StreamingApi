namespace Streaming.Api.Core.Services
{
    using System.Threading.Tasks;

    public interface ITweetStreamConnection
    {
        /// <summary>
        /// Specifically connects to the twitter sampled streaming endpoint.
        /// </summary>
        /// <returns></returns>
        Task ConnectToSampledStreamAsync();
    }
}
