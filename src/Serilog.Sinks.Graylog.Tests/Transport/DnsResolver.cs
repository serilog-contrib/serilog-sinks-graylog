using System.Net;
using System.Threading.Tasks;
using Serilog.Sinks.Graylog.Transport;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests.Transport
{
    public class DnsWrapperFixture
    {
        [Fact]
        public async Task Test()
        {
            var traget = new DnsWrapper();

            IPAddress[] actual = await traget.GetHostAddresses("github.com");

            Assert.NotEmpty(actual);
        }
    }
}