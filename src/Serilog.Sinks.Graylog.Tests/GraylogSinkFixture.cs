using System;
using System.Collections.Generic;
using Moq;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Graylog.Transport;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests
{
    public class GraylogSinkFixture
    {

        [Fact]
        public void WhenEmit_ThenSendData()
        {
            var gelfConverter = new Mock<IGelfConverter>();
            var transport = new Mock<ITransport>();

            var options = new GraylogSinkOptions
            {
                GelfConverter = gelfConverter.Object,
                Transport = transport.Object,
                HostnameOrAdress = "localhost"
            };

            GraylogSink target = new GraylogSink(options);

            //var fixture = new Fixture();

            //var logEvent = fixture.Create<LogEvent>();

            var logevent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Fatal, null,
                new MessageTemplate("O_o", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            var jobject = new JObject();

            gelfConverter.Setup(c => c.GetGelfJson(logevent, options)).Returns(jobject);

            target.Emit(logevent);

            gelfConverter.VerifyAll();

            transport.Verify(c => c.Send(jobject.ToString(Newtonsoft.Json.Formatting.None)));
        }
    }
}