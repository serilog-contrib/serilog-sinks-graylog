using AutoFixture;
using Serilog.Sinks.Graylog.Core.Transport.Udp;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests.Transport.Udp
{
    public class UdpTransportClientFixture
    {
        [Fact]
        public async Task TrySendSomeData()
        {
            var fixture = new Fixture();
            var bytes = fixture.CreateMany<byte>(128);

            var client = new UdpTransportClient(new GraylogSinkOptions
            {
                HostnameOrAddress = "127.0.0.1",
                Port = 3128
            });

            await client.Send(bytes.ToArray());
        }
    }
}
