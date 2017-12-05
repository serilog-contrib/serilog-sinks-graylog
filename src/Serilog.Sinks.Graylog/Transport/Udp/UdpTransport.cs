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
        private IPEndPoint hostEndPoint;

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
            var ipEndpoint = await GetOrCreateIpEndpoint().ConfigureAwait(false);
            foreach (var chunk in chunks)
            {
                await udpClient.SendAsync(chunk, chunk.Length, ipEndpoint).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get's or creats an IPEndPoint for the provided hostname Or Address
        /// </summary>
        /// <returns></returns>
        private async Task<IPEndPoint> GetOrCreateIpEndpoint()
        {
            if (hostEndPoint != null) return hostEndPoint;
            var ipAddresses = await Dns.GetHostAddressesAsync(hostnameOrAddress).ConfigureAwait(false);
            var ipAddress = ipAddresses.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
            if (ipAddress == null) throw new Exception($"Unable to resolve ip address for {hostnameOrAddress}");
            hostEndPoint = new IPEndPoint(ipAddress, port);
            return hostEndPoint;
        }

        public void Dispose()
        {
            var disposable = udpClient as IDisposable;
            disposable?.Dispose();
        }
    }
}