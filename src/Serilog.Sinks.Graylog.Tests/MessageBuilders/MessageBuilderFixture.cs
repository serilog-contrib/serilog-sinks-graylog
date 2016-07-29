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
        public void WhenGetSimpleEvent_ThenResult_ShouldBeExpected()
        {
            var options = new GraylogSinkOptions();
            var target = new GelfMessageBuilder("localhost", options);

            var date = DateTimeOffset.Now;

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

            LogEvent logEvent = LogEventSource.GetSimpleLogEvent(date);

            string expectedString = JsonConvert.SerializeObject(expected, Newtonsoft.Json.Formatting.None);
            string actual = target.Build(logEvent).ToString(Newtonsoft.Json.Formatting.None);
            //actual.ShouldBeEquivalentTo(expectedString);
        }

        [Fact]
        [Trait("Category", "Debug")]
        public void TryComplexEvent()
        {
            var options = new GraylogSinkOptions();
            var target = new GelfMessageBuilder("localhost", options);

            DateTimeOffset date = DateTimeOffset.Now;

            LogEvent logEvent = LogEventSource.GetComplexEvent(date);

            string actual = target.Build(logEvent).ToString(Newtonsoft.Json.Formatting.None);
        }


    }
}