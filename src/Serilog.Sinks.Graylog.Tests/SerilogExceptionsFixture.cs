using System;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Graylog.Core.Transport;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests
{
    public class SerilogExceptionsFixture
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void WhenUseSerilogExceptions_ThenExceptionDetailsShouldBeSent()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig
                //.Enrich.WithExceptionDetails()
                .WriteTo.Graylog(new GraylogSinkOptions
            {
                ShortMessageMaxLength = 50,
                MinimumLogEventLevel = LogEventLevel.Information,
                TransportType = TransportType.Http,
                Facility = "VolkovTestFacility",
                HostnameOrAddress = "http://logs.aeroclub.int",
                Port = 12201
            });

            var logger = loggerConfig.CreateLogger();

            try
            {
                throw new InvalidOperationException("Test exception");
            }
            catch (Exception e)
            {
                var test = new TestClass
                {
                    Id = 1,
                    Bar = new Bar
                    {
                        Id = 2,
                        Prop = "123"
                    },
                    TestPropertyOne = "1",
                    TestPropertyThree = "3",
                    TestPropertyTwo = "2"
                };

                logger.Error(e, "Exception {@entry}", test);
            }
        }
    }
}