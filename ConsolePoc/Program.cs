namespace SampledStreamClient
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var host = Startup.CreateHostBuilder();
            
            await host.RunConsoleAsync();

            return Environment.ExitCode;
        }
    }
}
