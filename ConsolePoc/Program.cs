namespace SampledStreamClient
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using Serilog;
    using Serilog.Events;
    using Streaming.Api.Core.Data;
    using Streaming.Api.Implementation.Data;

    class Program
    {
        // stream client for external application entry point
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }

        // responsible for startup of app - puts all the things together
        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                //.Enrich.WithMachineName()
                .WriteTo.Console();

            //Where does this go?
            Log.Logger = loggerConfiguration.CreateLogger();

            Log.Logger.Information("Application Starting");

            //
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .UseSerilog((hostContext, logConfig) =>
                {
                    logConfig.ReadFrom.Configuration(hostContext.Configuration);
                })
                // add di container for config
                .ConfigureServices((context, services) =>
                {
                    //services.AddSingleton(() =>
                    //{
                        //var restClientAutoLogConfiguration = new RestClientAutologConfiguration
                        //{
                        //    MessageTemplateForSuccess = "{Method} {Uri} responded {StatusCode}",
                        //    MessageTemplateForError = "{Method} {Uri} responded {StatusCode}",
                        //    LoggerConfiguration = loggerConfiguration,
                        //};
                        //IRestClient client = new RestClientAutolog(restClientAutoLogConfiguration);
                    //})
                    services.AddTransient<IDataService, DataService>();
                });

            return host;
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            /*
             * System.IO.FileNotFoundException: 'The configuration file 'appsettings.json' was not found and is not optional. The physical path is 'C:\Users\Denton\source\repos\StreamingApi\ConsolePoc\bin\Debug\net5.0\appsettings.json'.'
             */
            var configDir = Directory.GetCurrentDirectory();

            // TODO quick hack for now - but fix this.
            configDir = "c:\\users\\denton\\source\\repos\\streamingapi\\consolepoc";

            builder.SetBasePath(configDir)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}
