using FluentAssertions;
using Serilog.Events;
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
                HostnameOrAdress = "localhost",
                Port = 12201
            });

            var logger = loggerConfig.CreateLogger();
            logger.Should().NotBeNull();
        }
    }
}