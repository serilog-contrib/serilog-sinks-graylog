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

            while (true)
            {
                var line = Console.ReadLine();
                logger.Warning("some warning: {test}", line);
            }
        }
    }
}
