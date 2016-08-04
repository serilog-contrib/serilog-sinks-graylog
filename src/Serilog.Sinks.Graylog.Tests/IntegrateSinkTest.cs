using System;
using System.Text;
using Serilog.Events;
using Xunit;
using Serilog.Sinks;
using Serilog.Sinks.Graylog.Helpers;

namespace Serilog.Sinks.Graylog.Tests
{
    [Trait("Category", "Integration")]
    public class IntegrateSinkTest
    {
        [Fact]
        [Trait("Category", "Integration")]
        //[Fact]
        public void TestComplex()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                ShortMessageMaxLength = 50,
                MinimumLogEventLevel = LogEventLevel.Information,
                Facility = "VolkovTestFacility",
                HostnameOrAdress = "logs.aeroclub.int",
                Port = 12201
            });

            var logger = loggerConfig.CreateLogger();

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

            logger.Information("SomeComplexTestEntry {@test}", test);
        }

        [Fact]
        [Trait("Category", "Integration")]
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
                Id = 1,
                Bar = new Bar
                {
                    Id = 2
                },
                TestPropertyOne = "1",
                TestPropertyThree = "3",
                TestPropertyTwo = "2"
            };

            var logger = loggerConfig.CreateLogger();

            logger.Information("Some message {TestPropertyOne}{TestPropertyTwo}{TestPropertyThree}", test.TestPropertyOne, test.TestPropertyTwo, test.TestPropertyThree);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TestException()
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
                Id = 1,
                Bar = new Bar
                {
                    Id = 2
                },
                TestPropertyOne = "1",
                TestPropertyThree = "3",
                TestPropertyTwo = "2"
            };


            var logger = loggerConfig.CreateLogger();

            try
            {
                try
                {
                    throw new InvalidOperationException("Level One exception");
                }
                catch (Exception exc)
                {
                    throw new NotImplementedException("Nested Exception", exc);
                }
            }
            catch (Exception exc)
            {
                logger.Error(exc, "test exception with object {@test}", test);
            }
        }

    }


    public class Bar
    {
        public int Id { get; set; }
        public string Prop { get; set; }
    }

    public class TestClass
    {
        public int Id { get; set; }

        public string TestPropertyOne { get; set; }

        public Bar Bar { get; set; }

        public string TestPropertyTwo { get; set; }

        public string TestPropertyThree { get; set; }
    }
}