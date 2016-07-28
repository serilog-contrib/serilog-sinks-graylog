using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
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

            var date = DateTimeOffset.Now;
            var logEvent = new LogEvent(date, LogEventLevel.Information, null,
                new MessageTemplate("abcdef{TestProp}", new List<MessageTemplateToken>
                {
                    new TextToken("abcdef", 0),
                    new PropertyToken("TestProp", "zxc", alignment:new Alignment(AlignmentDirection.Left, 3))

                }), new List<LogEventProperty>
                {
                    new LogEventProperty("TestProp", new ScalarValue("zxc")),
                    new LogEventProperty("id", new ScalarValue("asd"))
                });

            var expected = new
            {
                facility = "GELF",
                full_message = "abcdef\"zxc\"",
                host= "localhost",
                level = 2,
                short_message= "abcdef\"zxc\"",
                timestamp = date.DateTime,
                version = "1.1",
                _stringLevel = "Information",
                _TestProp = "\"zxc\"",
                _id_ = "\"asd\""
            };

            var expectedString = JsonConvert.SerializeObject(expected, Newtonsoft.Json.Formatting.None);
            var actual = target.Build(logEvent).ToString(Newtonsoft.Json.Formatting.None);
            actual.ShouldBeEquivalentTo(expectedString);
        }
    }
}