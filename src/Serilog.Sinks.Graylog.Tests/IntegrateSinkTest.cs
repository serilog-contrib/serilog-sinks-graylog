using System;
using System.Text;
using Serilog.Events;
using Xunit;
using Serilog.Sinks;
using Serilog.Sinks.Graylog.Helpers;

namespace Serilog.Sinks.Graylog.Tests
{
    public class IntegrateSinkTest
    {
        [Fact]
        public void TestComplex()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                MinimumLogEventLevel = LogEventLevel.Information,
                Facility = "VolkovTestFacility",
                HostnameOrAdress = "logs.aeroclub.int",
                Port = 12201
            });

            var logger = loggerConfig.CreateLogger();

            var test2 = new TestClass
            {
                TestPropertyOne = "3",
                TestPropertyThree = "4",
                TestPropertyTwo = "5"
            };

            logger.Information("SomeComplexTestEntry {@test}", test2);
        }

        [Fact]
        public void TestSimple()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                MinimumLogEventLevel = LogEventLevel.Information,
                MessageGeneratorType = MessageIdGeneratortype.Timestamp,
                Facility = "VolkovTestFacility",
                HostnameOrAdress = "logs.aeroclub.int",
                Port = 12201
            });

            var test = new TestClass
            {
                TestPropertyOne = "1",
                TestPropertyThree = "3",
                TestPropertyTwo = "2"
            };

            var logger = loggerConfig.CreateLogger();

            var sbtemplate = "SomeTestEntry with guid {TestGuid}";
            var sb = new StringBuilder();
            for (int i = 0; i < 500; i++)
            {
                sb.AppendFormat($"sbtemplate{Guid.NewGuid()}");
            }
            var logstring = sb.ToString();
            logger.Information(logstring);
        }


    }




    public class TestClass
    {
        public string TestPropertyOne { get; set; }

        public string TestPropertyTwo { get; set; }

        public string TestPropertyThree { get; set; }
    }
}