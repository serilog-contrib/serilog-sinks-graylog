using System.Net;
using Serilog.Sinks.Graylog.Transport;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests.Transport
{
    public class DnsWrapperFixture
    {
        [Fact]
        public async void Test()
        {
            var traget = new DnsWrapper();

            IPAddress[] actual = await traget.GetHostAddresses("github.com");

            Assert.NotEmpty(actual);
        }
    }
}