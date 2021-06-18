using System;
using System.Threading.Tasks;

namespace Streaming.Api.Core.Services
{
    /// <summary>
    /// Updates the console periodically. Represents an interested stakeholder.
    /// </summary>
    public interface IConsoleUpdater
    {
        /// <summary>
        /// Updates the console window with a period defined by the parameter.
        /// </summary>
        Task StartUpdatesAsync(TimeSpan updateInterval);
    }
}
