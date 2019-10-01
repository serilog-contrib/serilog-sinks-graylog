using System.IO;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Transport;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests
{
    public class LoggerConfigurationGrayLogExtensionsFixture
    {
        [Fact]
        public void CanApplyExtension()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                MinimumLogEventLevel = LogEventLevel.Information,
                Facility = "VolkovTestFacility",
                HostnameOrAddress = "localhost",
                Port = 12201
            });

            var logger = loggerConfig.CreateLogger();
            logger.Should().NotBeNull();
        }

        [Fact]
        public void CanApplyExtensionWithIntegralParameterTypes()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog("localhost", 12201, TransportType.Udp,
                LogEventLevel.Information);

            var logger = loggerConfig.CreateLogger();
            logger.Should().NotBeNull();
        }


        //[Fact(Skip="Integration test")]
        [Fact]
        public void CanReadHostPropertyConfiguration()
        {
            //arrange
            //
            IConfigurationRoot configuration;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Serilog.Sinks.Graylog.Tests.Configurations.AppSettingsWithGraylogSinkContainingHostProperty.json"))
            {
                configuration = new ConfigurationBuilder()
                    .AddJsonStream(s)
                    .Build();
            }
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration, "Serilog")
                .CreateLogger();

            //act
            Log.Information("Hello {ApplicationName}.", "SerilogGraylogSink");

            //assert
        }

    }
}