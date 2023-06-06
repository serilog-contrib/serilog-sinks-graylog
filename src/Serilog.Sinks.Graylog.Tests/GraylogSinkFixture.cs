using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Transport;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests
{
    public class GraylogSinkFixture
    {

        [Fact(Skip = "This test not work anymore because IMessageBuilder gets from internal dictionary")]
        
        public void WhenEmit_ThenSendData()
        {
            var gelfConverter = new Mock<IGelfConverter>();
            var transport = new Mock<ITransport>();

            var options = new GraylogSinkOptions
            {
                GelfConverter = gelfConverter.Object,
                TransportType = TransportType.Udp,
                HostnameOrAddress = "localhost"
            };

            GraylogSink target = new GraylogSink(options);

            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Fatal, null,
                new MessageTemplate("O_o", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            var jObject = new JObject();
            transport.Setup(c => c.Send(jObject.ToString(Newtonsoft.Json.Formatting.None))).Returns(Task.CompletedTask);


            //gelfConverter.Setup(c => c.GetGelfJson(logEvent)).Returns(jObject);

            target.Emit(logEvent);

            gelfConverter.VerifyAll();

            transport.Verify(c => c.Send(It.IsAny<string>()));
        }
    }
}