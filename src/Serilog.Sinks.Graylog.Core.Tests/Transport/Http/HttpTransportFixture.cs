using AutoFixture;
using Moq;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.Graylog.Core.Transport.Http;
using System.Threading.Tasks;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests.Transport.Http
{
    public class HttpTransportFixture
    {
        private readonly Fixture _fixture;

        public HttpTransportFixture()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task WhenCallSend_ThenCallSendWithoutAnyChanges()
        {
            var transportClient = new Mock<ITransportClient<string>>();

            var target = new HttpTransport(transportClient.Object);

            var payload = _fixture.Create<string>();

            await target.Send(payload);

            transportClient.Verify(c => c.Send(payload), Times.Once);
        }
    }
}
