using System.Linq;
using System.Net;
using Ploeh.AutoFixture;
using Serilog.Sinks.Graylog.Transport.Udp;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests.Transport.Udp
{
    public class UdpTransportClientFixture
    {

        [Fact]
        public void TrySendSomeData()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, 3128);

            var fixture = new Fixture();
            var bytes = fixture.CreateMany<byte>(128);


            var client = new UdpTransportClient(ipLocalEndPoint);

            client.Send(bytes.ToArray());
        }
    }
}