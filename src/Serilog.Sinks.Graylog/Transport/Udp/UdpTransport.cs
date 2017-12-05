using System;
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
        private readonly bool skipDispose;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTransport"/> class.
        /// </summary>
        /// <param name="chunkConverter"></param>
        /// <param name="factory"></param>
        /// <param name="hostnameOrAddress"></param>
        /// <param name="port"></param>
        public UdpTransport(
            IDataToChunkConverter chunkConverter,
            string hostnameOrAddress,
            int port,
            Func<UdpClient> factory = null
        )
        {
            this.chunkConverter = chunkConverter;
            this.hostnameOrAddress = hostnameOrAddress;
            this.port = port;
            udpClient = factory?.Invoke() ?? new UdpClient();
            skipDispose = factory == null;
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
            foreach (var chunk in chunks)
            {
                await udpClient.SendAsync(chunk, chunk.Length, hostnameOrAddress, port).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            if (skipDispose) return;
            var disposable = udpClient as IDisposable;
            disposable?.Dispose();
        }
    }
}