namespace SampledStreamClient
{
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SampledStreamClient.Extensions;
    using Serilog;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Core.Data;
    using Streaming.Api.Core.Services;
    using Streaming.Api.Implementation.Configuration;
    using Streaming.Api.Implementation.Data;
    using Streaming.Api.Implementation.Services;

    internal class Startup
    {
        /// <summary>
        /// Responsible for configuration and middleware.
        /// </summary>
        internal static IHostBuilder CreateHostBuilder()
        {
            var loggerConfiguration = new LoggerConfiguration()
                //.ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                //.Enrich.WithMachineName()
                .WriteTo.Console();

            //Where does this go?
            Log.Logger = loggerConfiguration.CreateLogger();
            Log.Logger.Information("Application Starting");

            //return Host.CreateDefaultBuilder()
            //    .ConfigureServices(services =>
            //    {
            //        services.AddHostedService<Worker>();

            //        services.AddSingleton<IDataService, DataService>();

            //        services.AddScoped<ITweetProcessor, TweetProcessor>();
            //        services.AddScoped<ITweetStreamConnection, TweetStreamConnectionService>();

            //        services.AddTransient<IApiEnvironment, ApiEnvironment>();
            //    });

            return Host.CreateDefaultBuilder()
                .ConfigureServices();
        }

        ///// <summary>
        ///// Responsible for configuration and middleware.
        ///// </summary>
        //internal static IHostBuilder CreateHostBuilder(string[] args)
        //{
        //    var config = BuildConfig();

        //    var loggerConfiguration = new LoggerConfiguration()
        //        .ConfigureSerilog(config);

        //    //Where does this go?
        //    Log.Logger = loggerConfiguration.CreateLogger();
        //    Log.Logger.Information("Application Starting");

        //    var host = Host.CreateDefaultBuilder(args)
        //        .ConfigureLogging()
        //        .ConfigureServices();

        //    return host;
        //}

        static IConfigurationRoot BuildConfig()
        {
            var builder = new ConfigurationBuilder();

            /*
             * System.IO.FileNotFoundException: 'The configuration file 'appsettings.json' was not found and is not optional. The physical path is 'C:\Users\Denton\source\repos\StreamingApi\ConsolePoc\bin\Debug\net5.0\appsettings.json'.'
             */
            var configDir = Directory.GetCurrentDirectory();

            // TODO quick hack for now - but fix this.
            //configDir = "c:\\users\\denton\\source\\repos\\streamingapi\\consolepoc";

            builder.SetBasePath(configDir)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}

//TODO figure this stuff out - 

//namespace SampledStreamClient
//{
//    using System.IO;
//    using Microsoft.Extensions.Configuration;
//    using Microsoft.Extensions.DependencyInjection;
//    using Microsoft.Extensions.Hosting;
//    using SampledStreamClient.Extensions;
//    using SampledStreamClient.Services;
//    using Serilog;
//    using Streaming.Api.Core.Data;
//    using Streaming.Api.Implementation.Data;

//    internal class Startup
//    {
//        /// <summary>
//        /// Responsible for configuration and middleware.
//        /// </summary>
//        internal static IHostBuilder CreateHostBuilder()
//        {
//            var config = BuildConfig();

//            var loggerConfiguration = new LoggerConfiguration().ConfigureSerilog(config);

//            Log.Logger = loggerConfiguration.CreateLogger();
//            Log.Logger.Information("Application Starting");

//            var host = Host.CreateDefaultBuilder()
//                .ConfigureLogging()
//                .ConfigureServices();

//            Log.Logger.Information("Application started");

//            return host;
//        }

//        /// <summary>
//        /// Responsible for configuration and middleware.
//        /// </summary>
//        //internal static IHostBuilder CreateHostBuilder(string[] args)
//        //{
//        //    var config = BuildConfig();

//        //    var loggerConfiguration = new LoggerConfiguration().ConfigureSerilog(config);

//        //    Log.Logger = loggerConfiguration.CreateLogger();
//        //    Log.Logger.Information("Application Starting");

//        //    var host = Host.CreateDefaultBuilder(args)
//        //        .ConfigureLogging()
//        //        .ConfigureServices();

//        //    return host;
//        //}

//        static IConfigurationRoot BuildConfig()
//        {
//            var builder = new ConfigurationBuilder();

//            var configDir = Directory.GetCurrentDirectory();

//            builder.SetBasePath(configDir)
//                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                .AddEnvironmentVariables();

//            return builder.Build();
//        }
//    }
//}
