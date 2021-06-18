using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Api.Core.Services
{
    using Streaming.Api.Models;

    public interface IConsoleUpdater
    {
        /// <summary>
        /// Updates the console window with a period defined by the parameter.
        /// </summary>
        void StartUpdates(TimeSpan updateInterval);

        /// <summary>
        /// Updates the console window with a period defined by the parameter.
        /// </summary>
        Task StartUpdatesAsync(TimeSpan updateInterval);
    }

    public interface IReportService
    {
        Task<TweetStatsReport> QueryStatisticsAsync();
    }
}
