using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog.Events;
using Xunit;
using Serilog.Sinks.Graylog.Core.Helpers;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.Graylog.Tests.ComplexIntegrationTest;

namespace Serilog.Sinks.Graylog.Tests
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public enum TestEnumOne
    {
        One, Two, Three
    }

    [Trait("Category", "Integration")]
    public class IntegrateSinkTestWithUdp
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void VerifyLoggerVerbocity()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                ShortMessageMaxLength = 50,
                MinimumLogEventLevel = LogEventLevel.Information,
                Facility = "edox-accounts",
                HostnameOrAddress = "msa-edor02-lg01",
                Port = 12209,
                UseGzip = false,
                JsonSerializerOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                },
                TransportType = TransportType.Udp
            });

            var logger = loggerConfig.CreateLogger();

            var test = new TestClass
            {
                Id = 1,
                SomeTestDateTime = DateTime.UtcNow,
                Bar = new Bar
                {
                    Id = 2,
                    Prop = "whirlwind",
                    TestBarBooleanProperty = false,
                    EnumVal = TestEnumOne.Three
                },
                TestClassBooleanProperty = true,
                TestPropertyOne = "1",
                TestPropertyThree = "3",
                TestPropertyTwo = "2",
                EnumVal = TestEnumOne.Three
            };
            logger.Error("SomeComplexTestEntry {@test}", test);
            logger.Information("SomeComplexTestEntry {@test}", test);
            logger.Fatal("SomeComplexTestEntry {@test}", test);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TestArrays()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                ShortMessageMaxLength = 50,
                MinimumLogEventLevel = LogEventLevel.Information,
                Facility = "edox-accounts",
                HostnameOrAddress = "msa-edor02-lg01",
                Port = 12209,
                UseGzip = false,
                ParseArrayValues = true,
                JsonSerializerOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                },
                TransportType = TransportType.Udp
            });

            var logValue = new
            {
                Bars = new List<Bar>
                {
                    new Bar
                    {
                        Id = 1,
                        Prop = "1",
                        TestBarBooleanProperty = true
                    },
                    new Bar
                    {
                        Id = 2,
                        Prop = "2",
                        TestBarBooleanProperty = false
                    },
                    new Bar
                    {
                        Id = 3,
                        Prop = "3",
                        TestBarBooleanProperty = true
                    }
                }
            };
            
            var logger = loggerConfig.CreateLogger();
            
            logger.Error("SomeComplexTestEntry {@test}", logValue);

        }
        
        [Fact]
        [Trait("Category", "Integration")]
        public void TestArrays2()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                ShortMessageMaxLength = 50,
                MinimumLogEventLevel = LogEventLevel.Information,
                Facility = "edox-accounts",
                HostnameOrAddress = "msa-edor02-lg01",
                Port = 12209,
                UseGzip = false,
                ParseArrayValues = true,
                IncludeMessageTemplate = true,
                JsonSerializerOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                },
                TransportType = TransportType.Udp
            });

            var test = new[]
            {
                new Bar
                {
                    Id = 1,
                    Prop = "1",
                    TestBarBooleanProperty = true
                },
                new Bar
                {
                    Id = 2,
                    Prop = "2",
                    TestBarBooleanProperty = false
                },
                new Bar
                {
                    Id = 3,
                    Prop = "3",
                    TestBarBooleanProperty = true
                }
            };
            
            var logger = loggerConfig.CreateLogger();
            
            logger.Error("SomeComplexTestEntry {@test}", test);

        }        
        
        [Fact]
        [Trait("Category", "Integration")]
        public void TestDictionary()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                ShortMessageMaxLength = 50,
                MinimumLogEventLevel = LogEventLevel.Information,
                HostnameOrAddress = "msa-edor02-lg01",
                Port = 12209,
                UseGzip = false,
                ParseArrayValues = false,
                IncludeMessageTemplate = true,
                JsonSerializerOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                },
                TransportType = TransportType.Udp
            });

            var response = new Dictionary<int, Bar>()
            {
                [0] = new Bar
                {
                    
                    Id = 1,
                    Prop = "1",
                    TestBarBooleanProperty = true,
                    EnumVal = TestEnumOne.One
                },
                [1] = new Bar
                {
                    Id = 2,
                    Prop = "2",
                    TestBarBooleanProperty = false,
                    EnumVal = TestEnumOne.Two
                },
                [2] = new Bar
                {
                    Id = 3,
                    Prop = "3",
                    TestBarBooleanProperty = true,
                    EnumVal = TestEnumOne.Three
                }
            };
            
            var logger = loggerConfig.CreateLogger();
            
            logger.Information("Ответ: {@CommandResponse}", (object) response);
        }      
        
        [Fact]
        [Trait("Category", "Integration")]
        public void TestComplex()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                ShortMessageMaxLength = 50,
                MinimumLogEventLevel = LogEventLevel.Information,
                Facility = "VolkovTestFacility",
                HostnameOrAddress = "logs.aeroclub.int",
                Port = 12201
            });

            var logger = loggerConfig.CreateLogger();

            var test = new TestClass
            {
                Id = 1,
                Type = "UDP",
                SomeTestDateTime = DateTime.UtcNow,
                Bar = new Bar
                {
                    Id = 2,
                    Prop = "123",
                    TestBarBooleanProperty = false
                    
                },
                TestClassBooleanProperty = true,
                TestPropertyOne = "1",
                TestPropertyThree = "3",
                TestPropertyTwo = "2"
            };

            logger.Information("SomeComplexTestEntry {@test}", test);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public Task SendManyMessages()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Clear();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
            var profiles = fixture.CreateMany<Profile>(10).ToList();

            foreach (var profile in profiles)
            {
                profile.Type = "UDP";
            }

            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                MinimumLogEventLevel = LogEventLevel.Information,
                MessageGeneratorType = MessageIdGeneratorType.Md5,
                Facility = "VolkovTestFacility",
                HostnameOrAddress = "logs.aeroclub.int",
                Port = 12201
            });

            var logger = loggerConfig.CreateLogger();

            var tasks = profiles.Select(c => 
            {
                return Task.Run(() => logger.Information("TestSend {@BattleProfile}", c));
            });


            return Task.WhenAll(tasks.ToArray());
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TestSimple()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Clear();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
            var profile = fixture.Create<Profile>();

            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                MinimumLogEventLevel = LogEventLevel.Information,
                MessageGeneratorType = MessageIdGeneratorType.Timestamp,
                Facility = "VolkovTestFacility",
                HostnameOrAddress = "logs.aeroclub.int",
                Port = 12201
            });

            var logger = loggerConfig.CreateLogger();

            logger.Information("battle profile:  {@BattleProfile}", profile);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void IncludeTemplate()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Clear();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
            var profile = fixture.Create<Profile>();

            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                MinimumLogEventLevel = LogEventLevel.Information,
                MessageGeneratorType = MessageIdGeneratorType.Timestamp,
                Facility = "VolkovTestFacility",
                HostnameOrAddress = "logs.aeroclub.int",
                Port = 12201,
                IncludeMessageTemplate = true
            });

            var logger = loggerConfig.CreateLogger();

            logger.Information("battle profile:  {@BattleProfile}", profile);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TestException()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                MinimumLogEventLevel = LogEventLevel.Information,
                MessageGeneratorType = MessageIdGeneratorType.Timestamp,
                TransportType = TransportType.Udp,
                Facility = "VolkovTestFacility",
                HostnameOrAddress = "logs.aeroclub.int",
                Port = 12201
            });

            var test = new TestClass
            {
                Id = 1,
                SomeTestDateTime = DateTime.UtcNow,
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

        [Fact]
        [Trait("Category", "Integration")]
        public void SerializeEvent()
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig.WriteTo.Graylog(new GraylogSinkOptions
            {
                MinimumLogEventLevel = LogEventLevel.Information,
                MessageGeneratorType = MessageIdGeneratorType.Timestamp,
                TransportType = TransportType.Udp,
                Facility = "VolkovTestFacility",
                HostnameOrAddress = "logs.aeroclub.int",
                Port = 12201
            });

            var payload = new Event("123");

            var logger = loggerConfig.CreateLogger();

            logger.Information("test event {@payload}", payload);
        }
    }

    public class Event
    {
        public Event(string eventId)
        {
            EventId = eventId;
            Timestamp = DateTime.UtcNow;
        }

        public DateTime Timestamp { get; set; }

        public string EventId { get; set; }
    }

    public class Bar
    {
        public int Id { get; set; }
        public string Prop { get; set; }

        public bool TestBarBooleanProperty { get; set; }
        public TestEnumOne EnumVal { get; set; }
    }

    public class TestClass
    {
        public int Id { get; set; }

        public bool TestClassBooleanProperty { get; set; }

        public string TestPropertyOne { get; set; }

        public Bar Bar { get; set; }

        public string TestPropertyTwo { get; set; }

        public string TestPropertyThree { get; set; }
        public DateTime SomeTestDateTime { get; set; }
        public string Type { get; set; }
        public TestEnumOne EnumVal { get; set; }
    }
}