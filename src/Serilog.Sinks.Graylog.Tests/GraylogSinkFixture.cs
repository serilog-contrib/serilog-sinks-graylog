﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        [Fact(Skip = "This test not work anymore because IMessageBuilder gets from internal dictionary")]

        public void WhenEmit_ThenSendData()
        {
            var gelfConverter = new Mock<IGelfConverter>();
            var transport = new Mock<ITransport>();

            var options = new GraylogSinkOptions
            {
                GelfConverter = gelfConverter.Object,
                TransportType = TransportType.Udp,
                HostnameOrAddress = "localhost",
            };

            GraylogSink target = new GraylogSink(options);

            var logevent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Fatal, null,
                new MessageTemplate("O_o", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            var jobject = new JObject();
            transport.Setup(c => c.Send(jobject.ToString(Newtonsoft.Json.Formatting.None))).Returns(Task.CompletedTask);


            gelfConverter.Setup(c => c.GetGelfJson(logevent)).Returns(jobject);

            target.Emit(logevent);

            gelfConverter.VerifyAll();

            transport.Verify(c => c.Send(It.IsAny<string>()));
        }

        [Fact]
        public void WhenError_HandlesGracefully()
        {
            var gelfConverter = new Mock<IGelfConverter>();
            gelfConverter.Setup(c => c.GetGelfJson(It.IsAny<LogEvent>())).Returns(new JObject());

            var transport = new Mock<ITransport>();
            transport.Setup(c => c.Send(It.IsAny<string>())).Throws(new Exception("Send Failed"));

            var options = new GraylogSinkOptions
            {
                GelfConverter = gelfConverter.Object,
                TransportType = TransportType.Udp,
                HostnameOrAddress = "localhost",
                ThrowOnSendError = false
            };

            var sut = new GraylogSink(options, () => transport.Object);

            var logevent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Fatal, null,
                new MessageTemplate("O_o", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            sut.Emit(logevent);
        }

        [Fact]
        public void WhenError_RethrowsError()
        {
            var gelfConverter = new Mock<IGelfConverter>();
            gelfConverter.Setup(c => c.GetGelfJson(It.IsAny<LogEvent>())).Returns(new JObject());

            var transport = new Mock<ITransport>();
            transport.Setup(c => c.Send(It.IsAny<string>())).Throws(new Exception("Send Failed"));

            var options = new GraylogSinkOptions
            {
                GelfConverter = gelfConverter.Object,
                TransportType = TransportType.Udp,
                HostnameOrAddress = "localhost",
                ThrowOnSendError = true
            };

            var sut = new GraylogSink(options, () => transport.Object);

            var logevent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Fatal, null,
                new MessageTemplate("O_o", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            Assert.Throws<Exception>(() =>
            {
                sut.Emit(logevent);
            });
        }
    }
}