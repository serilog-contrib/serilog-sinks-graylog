using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using Serilog.Sinks.Graylog.Transport;
using Serilog.Sinks.Graylog.Transport.Http;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests.Transport.Http
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