using FluentAssertions;
using Serilog.Sinks.Graylog.Core.Transport.Tcp;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests.Transport.Tcp
{
    using Core.Transport;

    public class TcpTransportClientFixture
    {
        [Fact]
        public void DisposeBeforeAnySendDoesNotThrow()
        {
            var client = new TcpTransportClient(new GraylogSinkOptions
            {
                HostnameOrAddress = "127.0.0.1",
                Port = 12201
            }, new DnsWrapper());

            Action dispose = client.Dispose;

            dispose.Should().NotThrow();
        }

        [Fact]
        public async Task ReconnectsAfterServerResetsConnection()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;

            using var client = new TcpTransportClient(new GraylogSinkOptions
            {
                HostnameOrAddress = "127.0.0.1",
                Port = port
            }, new DnsWrapper());

            await client.Send(new byte[] { 1 });

            // Close the first connection with a reset (linger 0), the way a load balancer drops it.
            using (var first = await listener.AcceptTcpClientAsync())
            {
                first.LingerState = new LingerOption(true, 0);
            }

            // The write that detects the dead socket may fail; a later send must open a new connection.
            var acceptSecond = listener.AcceptTcpClientAsync();
            for (var attempt = 0; attempt < 10 && !acceptSecond.IsCompleted; attempt++)
            {
                try
                {
                    await client.Send(new byte[] { 2 });
                }
                catch (IOException)
                {
                }
                catch (SocketException)
                {
                }

                await Task.WhenAny(acceptSecond, Task.Delay(200));
            }

            acceptSecond.IsCompleted.Should().BeTrue("the client should reconnect after the connection was reset");
            using var second = await acceptSecond;
            listener.Stop();
        }
    }
}
