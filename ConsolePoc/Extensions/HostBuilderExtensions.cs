namespace SampledStreamClient.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;
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
        /// Configures logging, by clearing the default logging and using Serilog.
        /// </summary>
        internal static IHostBuilder ConfigureLogging(this IHostBuilder self)
        {
            return self.ConfigureLogging(logging => { logging.ClearProviders(); })
                .UseSerilog((hostContext, logConfig) =>
                {
                    logConfig.ReadFrom.Configuration(hostContext.Configuration);
                });
        }

        /// <summary>
        /// Configures services for the app
        /// </summary>
        internal static IHostBuilder ConfigureServices(this IHostBuilder self)
        {
            return self.ConfigureServices((_, services) =>
            {
                services.AddHostedService<Worker>();
                //services.AddHostedService<StreamReaderWorker>();
                //services.AddHostedService<UpdaterWorker>();
                //services.AddHostedService<TweetProcessorWorker>();

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
