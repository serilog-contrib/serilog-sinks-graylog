using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace TestApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .Build();
            
            Logger logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(configuration)
                .CreateLogger();

            logger.Warning("some warning: {test}", "test message");


            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}
