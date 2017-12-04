using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Serilog.Sinks.Graylog.Extensions;

namespace Serilog.Sinks.Graylog.Transport.Udp
{
    public sealed class UdpTransport : ITransport
    {
        private readonly IDataToChunkConverter chunkConverter;
        private readonly string hostnameOrAddress;
        private readonly int port;
        private readonly UdpClient udpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTransport"/> class.
        /// </summary>
        /// <param name="chunkConverter"></param>
        /// <param name="hostnameOrAddress"></param>
        /// <param name="port"></param>
        public UdpTransport(IDataToChunkConverter chunkConverter, string hostnameOrAddress, int port)
        {
            this.chunkConverter = chunkConverter;
            this.hostnameOrAddress = hostnameOrAddress;
            this.port = port;
            udpClient = new UdpClient();
        }

        /// <inheritdoc />
        /// <summary>
        /// Sends the specified target.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="T:System.ArgumentException">message was too long</exception>
        /// <exception cref="T:System.ArgumentException">message was too long</exception>
        public async Task Send(string message)
        {
            var compressedMessage = message.Compress();
            var chunks = chunkConverter.ConvertToChunks(compressedMessage);
            var ipAddreses = await Dns.GetHostAddressesAsync(hostnameOrAddress).ConfigureAwait(false);
            var ipAddress = ipAddreses.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
            var ipEndpoint = new IPEndPoint(ipAddress, port);

            foreach (var chunk in chunks)
            {
                await udpClient.SendAsync(chunk, chunk.Length, ipEndpoint).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            var disposable = udpClient as IDisposable;
            disposable?.Dispose();
        }
    }
}