using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Graylog.Core.MessageBuilders;
using Serilog.Sinks.Graylog.Tests;
using System;
using System.Collections.Generic;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests.MessageBuilders
{
    public class GelfMessageBuilderFixture
    {
        [Fact]
        [Trait("Category", "Debug")]
        public void TryComplexEvent()
        {
            var options = new GraylogSinkOptions();
            var target = new GelfMessageBuilder("localhost", options);

            DateTimeOffset date = DateTimeOffset.Now;

            LogEvent logEvent = LogEventSource.GetComplexEvent(date);

            //string actual = target.Build(logEvent).ToString(Newtonsoft.Json.Formatting.None);
        }

        [Fact]
        public void GetSimpleLogEvent_GraylogSinkOptionsContainsHost_ReturnsOptionsHost()
        {
            //arrange
            GraylogSinkOptions options = new()
            {
                HostnameOverride = "my_host"
            };
            GelfMessageBuilder messageBuilder = new("localhost", options);
            DateTime date = DateTime.UtcNow;
            string expectedHost = "my_host";

            //act
            LogEvent logEvent = LogEventSource.GetSimpleLogEvent(date);
            var actual = messageBuilder.Build(logEvent);
            string actualHost = actual["host"].AsValue().ToString();

            //assert
            Assert.Equal(expectedHost, actualHost);
        }


        [Fact]
        public static void WhenTryCreateLogEventWithNullKeyOrValue_ThenThrow()
        {
            //If in future this test fail then should add check for null in GelfMessageBuilder
            Assert.Throws<ArgumentNullException>(() =>
            {
                var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null,
                    new MessageTemplate("abcdef{TestProp}", new List<MessageTemplateToken>
                    {
                        new TextToken("abcdef", 0),
                        new PropertyToken("TestProp", "zxc", alignment: new Alignment(AlignmentDirection.Left, 3))

                    }), new List<LogEventProperty>
                    {
                        new("TestProp", new ScalarValue("zxc")),
                        new("id", new ScalarValue("asd")),
                        new("Oo", null),
                        new(null, null),
                        new("StructuredProperty",
                            new StructureValue(new List<LogEventProperty>
                            {
                                new("id", new ScalarValue(1)),
                                new("_TestProp", new ScalarValue(3)),
                            }, "TypeTag"))
                    });
            });
        }
    }
}
