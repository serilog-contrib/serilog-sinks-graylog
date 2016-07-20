using System;
using Serilog.Events;
using Xunit;
using Serilog.Sinks;

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
                BatchSizeLimit = 1,
                MinimumLogEventLevel = LogEventLevel.Information,
                Period = TimeSpan.FromSeconds(1),
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
                BatchSizeLimit = 1,
                MinimumLogEventLevel = LogEventLevel.Information,
                Period = TimeSpan.FromSeconds(1),
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

            logger.Information("SomeTestEntry {TestPropertyOne} {TestPropertyTwo} {TestPropertyThree}", test.TestPropertyOne, test.TestPropertyTwo, test.TestPropertyThree);
        }
    }


    public class TestClass
    {
        public string TestPropertyOne { get; set; }

        public string TestPropertyTwo { get; set; }

        public string TestPropertyThree { get; set; }
    }
}