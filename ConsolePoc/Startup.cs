namespace SampledStreamClient
{
    using Microsoft.Extensions.Hosting;
    using SampledStreamClient.Extensions;
    using Serilog;

    internal class Startup
    {
        /// <summary>
        /// Responsible for configuration and middleware.
        /// </summary>
        internal static IHostBuilder CreateHostBuilder()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console();

            Log.Logger = loggerConfiguration.CreateLogger();
            Log.Logger.Information("Application Starting");

            return Host.CreateDefaultBuilder()
                .ConfigureServices();
        }
    }
}