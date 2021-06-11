namespace SampledStreamClient.Extensions
{
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// Provides extensions methods for <see cref="LoggerConfiguration"/>
    /// </summary>
    internal static class LoggerConfigurationExtensions
    {
        /// <summary>
        /// Configures the Serilog logger from provided configuration
        /// </summary>
        internal static LoggerConfiguration ConfigureSerilog(
            this LoggerConfiguration self,
            IConfigurationRoot configRoot)
        {
            return self//.ReadFrom.Configuration(configRoot)
                .Enrich.FromLogContext()
                //.Enrich.WithMachineName()
                //.WriteTo.Logger(new OutputLogger())
                .WriteTo.Console();
        }
    }

    internal class OutputLogger : ILogger
    {
        public void Write(LogEvent logEvent)
        {
            foreach (var property in logEvent.Properties)
            {
                System.Diagnostics.Debug.WriteLine(property.Key);
                System.Diagnostics.Debug.WriteLine(property.Value.ToString());
            }
        }
    }
}