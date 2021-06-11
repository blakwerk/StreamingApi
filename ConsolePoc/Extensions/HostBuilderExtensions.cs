namespace SampledStreamClient.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using RestSharp;
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
            return self.ConfigureServices((context, services) =>
            {
                services.AddHostedService<Worker>();

                services.AddTransient<IApiEnvironment, ApiEnvironment>();
                services.AddTransient<IClientFactory, ClientFactory>();

                services.AddScoped<ITweetStreamConnection, TweetStreamConnectionService>();

                services.AddSingleton<IDataService, DataService>();
            });
        }
    }
}
