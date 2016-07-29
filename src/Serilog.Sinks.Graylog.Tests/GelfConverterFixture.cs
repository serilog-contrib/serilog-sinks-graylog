using System;
using System.Collections.Generic;
using Moq;
using Serilog.Sinks.Graylog.MessageBuilder;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests
{
    public class GelfConverterFixture
    {
        [Fact]
        public void WhenLogEvent_ThenMessageBuilderShouldBeCalled()
        {
            var errorBuilder = new Mock<IMessageBuilder>();
            var messageBuilder = new Mock<IMessageBuilder>();

            var messageBuilders = new Dictionary<BuilderType, IMessageBuilder>
            {
                [BuilderType.Exception] = errorBuilder.Object,
                [BuilderType.Message] = messageBuilder.Object
            };

            GelfConverter target = new GelfConverter("anyHost", new GraylogSinkOptions(), messageBuilders);

            var simpleEvent = LogEventSource.GetSimpleLogEvent(DateTimeOffset.Now);

            target.GetGelfJson(simpleEvent);

            errorBuilder.Verify(c => c.Build(simpleEvent), Times.Never);
            messageBuilder.Verify(c => c.Build(simpleEvent), Times.Once);
        }

        [Fact]
        public void WhenLogErrorEvent_ThenErrorMessageBuilderShouldBeCalled()
        {
            var errorBuilder = new Mock<IMessageBuilder>();
            var messageBuilder = new Mock<IMessageBuilder>();

            var messageBuilders = new Dictionary<BuilderType, IMessageBuilder>
            {
                [BuilderType.Exception] = errorBuilder.Object,
                [BuilderType.Message] = messageBuilder.Object
            };

            GelfConverter target = new GelfConverter("anyHost", new GraylogSinkOptions(), messageBuilders);

            var simpleEvent = LogEventSource.GetErrorEvent(DateTimeOffset.Now);

            target.GetGelfJson(simpleEvent);

            errorBuilder.Verify(c => c.Build(simpleEvent), Times.Once);
            messageBuilder.Verify(c => c.Build(simpleEvent), Times.Never);
        }
    }
}