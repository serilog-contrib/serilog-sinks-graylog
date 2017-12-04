using System;
using FluentAssertions;
using Serilog.Sinks.Graylog.MessageBuilders;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests.MessageBuilders
{
    public class ExceptionMessageBuilderFixture
    {
        [Fact]
        public void WhenCreateException_ThenBuildShoulNotThrow()
        {
            var options = new GraylogSinkOptions();

            var exceptionBuilder = new ExceptionMessageBuilder("localhost", options);

            Exception testExc = null;

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
                testExc = exc;
            }


            var date = DateTimeOffset.Now;
            var logEvent = LogEventSource.GetExceptionLogEvent(date, testExc);

            var obj = exceptionBuilder.Build(logEvent);

            obj.Should().NotBeNull();
        }
    }
}