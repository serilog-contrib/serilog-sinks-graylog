using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Graylog.MessageBuilders;
using Serilog.Sinks.Graylog.MessageBuilders.PropertyNaming;
using System;
using System.Collections.Generic;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests.MessageBuilders
{
    public class PropertyNamingFixture
    {
        [Fact]
        public void WhenCamelCasingPropertyNames_ShouldCamelCase()
        {
            var sut = new CamelCasePropertyNamingStrategy();
            Assert.Equal("longPropertyName", sut.GetPropertyName("LongPropertyName"));
            Assert.Equal("short", sut.GetPropertyName("Short"));
        }

        [Fact]
        public void WhenNoOpPropertyNames_ShouldIgnore()
        {
            var sut = new NoOpPropertyNamingStrategy();
            Assert.Equal("LongPropertyName", sut.GetPropertyName("LongPropertyName"));
            Assert.Equal("Short", sut.GetPropertyName("Short"));
        }

        [Fact]
        public void WhenMessageBuilding_ShouldCamelCasePropertyNames()
        {
            var sut = new GelfMessageBuilder("localhost", new GraylogSinkOptions
            {
                PropertyNamingStrategy = new CamelCasePropertyNamingStrategy(),
            });

            var @event = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null,
            new MessageTemplate("abcdef{TestProp}", new List<MessageTemplateToken>
            {
                new TextToken("abcdef", 0),
                new PropertyToken("LongPropertyName", "zxc", alignment:new Alignment(AlignmentDirection.Left, 3)),
                new PropertyToken("Short", "zxc", alignment:new Alignment(AlignmentDirection.Left, 3))

            }), new List<LogEventProperty>
            {
                new LogEventProperty("LongPropertyName", new ScalarValue("zxc")),
                new LogEventProperty("Short", new ScalarValue("zxc")),
                new LogEventProperty("id", new ScalarValue("asd"))
            });

            var actual = sut.Build(@event);
            Assert.Null(actual.Property("_LongPropertyName"));
            Assert.Null(actual.Property("_Short"));
            Assert.NotNull(actual.Property("_longPropertyName"));
            Assert.NotNull(actual.Property("_short"));
        }

        [Fact]
        public void WhenMessageBuilding_ShouldIgnorePropertyNames()
        {
            var sut = new GelfMessageBuilder("localhost", new GraylogSinkOptions
            {
                PropertyNamingStrategy = new NoOpPropertyNamingStrategy(),
            });

            var @event = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null,
            new MessageTemplate("abcdef{TestProp}", new List<MessageTemplateToken>
            {
                new TextToken("abcdef", 0),
                new PropertyToken("LongPropertyName", "zxc", alignment:new Alignment(AlignmentDirection.Left, 3)),
                new PropertyToken("Short", "zxc", alignment:new Alignment(AlignmentDirection.Left, 3))

            }), new List<LogEventProperty>
            {
                new LogEventProperty("LongPropertyName", new ScalarValue("zxc")),
                new LogEventProperty("Short", new ScalarValue("zxc")),
                new LogEventProperty("id", new ScalarValue("asd"))
            });

            var actual = sut.Build(@event);
            Assert.Null(actual.Property("_longPropertyName"));
            Assert.Null(actual.Property("_short"));
            Assert.NotNull(actual.Property("_LongPropertyName"));
            Assert.NotNull(actual.Property("_Short"));
        }
    }
}
