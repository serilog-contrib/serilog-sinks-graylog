using Serilog.Sinks.Graylog.Core.Transport;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests.Transport
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
