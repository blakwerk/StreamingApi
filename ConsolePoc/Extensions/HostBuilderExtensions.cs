namespace SampledStreamClient.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Core.Data;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Implementation.Configuration;
    using Streaming.Api.Implementation.Data;
    using Streaming.Api.Implementation.Services;

    /// <summary>
    /// Provides extension methods for <see cref="IHostBuilder"/>
    /// </summary>
    internal static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures services for the app
        /// </summary>
        internal static IHostBuilder ConfigureServices(this IHostBuilder self)
        {
            return self.ConfigureServices((_, services) =>
            {
                services.AddHostedService<Worker>();

                services.AddSingleton<ITweetProcessor, TweetProcessor>();
                services.AddSingleton<IDataService, DataService>();

                services.AddScoped<IReportService, ReportService>();
                services.AddScoped<IConsoleUpdater, ConsoleUpdater>();
                services.AddScoped<ITweetStreamConnection, TweetStreamConnectionService>();

                services.AddTransient<IApiEnvironment, ApiEnvironment>();
            });
        }
    }
}
