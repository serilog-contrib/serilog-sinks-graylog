using Serilog.Events;
using Serilog.Sinks.Graylog.Core.MessageBuilders;
using Serilog.Sinks.Graylog.Tests;
using System;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests.MessageBuilders
{
    public class ExceptionMessageBuilderFixture
    {
        [Fact]
        public void WhenCreateException_ThenBuildShoulNotThrow()
        {
            var options = new GraylogSinkOptions();

            ExceptionMessageBuilder exceptionBuilder = new("localhost", options);

            Exception testExc = null;

            try
            {
                try
                {
                    throw new InvalidOperationException("Level One exception");
                } catch (Exception exc)
                {
                    throw new NotImplementedException("Nested Exception", exc);
                }
            } catch (Exception exc)
            {
                testExc = exc;
            }


            DateTimeOffset date = DateTimeOffset.Now;
            LogEvent logEvent = LogEventSource.GetExceptionLogEvent(date, testExc);

            //JObject obj = exceptionBuilder.Build(logEvent);

            //obj.Should().NotBeNull();
        }
    }
}
