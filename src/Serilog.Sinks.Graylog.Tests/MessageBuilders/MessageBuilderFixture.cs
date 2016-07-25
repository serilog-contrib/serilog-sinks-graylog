using System;
using System.Collections.Generic;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;
using Serilog.Sinks.Graylog.MessageBuilders;

namespace Serilog.Sinks.Graylog.Tests.MessageBuilders
{
    public class GelfMessageBuilderFixture
    {
        [Fact]
        public void Test()
        {
            var options = new GraylogSinkOptions();
            var target = new GelfMessageBuilder("localhost", options);

            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null,
                new MessageTemplate("abcdef{TestProp}", new List<MessageTemplateToken>
                {
                    new TextToken("abcdef", 0),
                    new PropertyToken("TestProp", "zxc", alignment:new Alignment(AlignmentDirection.Left, 3))

                }), new List<LogEventProperty>
                {
                    new LogEventProperty("TestProp", new ScalarValue("zxc"))
                });

            target.Build(logEvent);
        }
    }
}