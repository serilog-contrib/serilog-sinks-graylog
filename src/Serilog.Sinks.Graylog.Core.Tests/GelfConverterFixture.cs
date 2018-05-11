using System;
using System.Collections.Generic;
using Moq;
using Serilog.Sinks.Graylog.Core.MessageBuilders;
using Serilog.Sinks.Graylog.Tests;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests
{
    public class GelfConverterFixture
    {
        [Fact]
        public void WhenLogEvent_ThenMessageBuilderShouldBeCalled()
        {
            var errorBuilder = new Mock<IMessageBuilder>();
            var messageBuilder = new Mock<IMessageBuilder>();

            var messageBuilders = new Dictionary<BuilderType, Lazy<IMessageBuilder>>
            {
                [BuilderType.Exception] = new Lazy<IMessageBuilder>(() => errorBuilder.Object),
                [BuilderType.Message] = new Lazy<IMessageBuilder>(() => messageBuilder.Object)
            };

            GelfConverter target = new GelfConverter(messageBuilders);

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

            var messageBuilders = new Dictionary<BuilderType, Lazy<IMessageBuilder>>
            {
                [BuilderType.Exception] = new Lazy<IMessageBuilder>(() => errorBuilder.Object),
                [BuilderType.Message] = new Lazy<IMessageBuilder>(() => messageBuilder.Object)
            };

            GelfConverter target = new GelfConverter(messageBuilders);

            var simpleEvent = LogEventSource.GetErrorEvent(DateTimeOffset.Now);

            target.GetGelfJson(simpleEvent);

            errorBuilder.Verify(c => c.Build(simpleEvent), Times.Once);
            messageBuilder.Verify(c => c.Build(simpleEvent), Times.Never);
        }
    }
}