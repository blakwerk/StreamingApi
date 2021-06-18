namespace Streaming.Api.Core.Services
{
    using System.Threading.Tasks;
    using Streaming.Api.Models;

    /// <summary>
    /// Represents a service built to generate reports from data - whether
    /// live or replicated data.
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Queries the processed tweets datastore for tweet report metadata.
        /// </summary>
        Task<TweetStatsReport> QueryStatisticsAsync();
    }
}