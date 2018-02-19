using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Serilog.Sinks.Graylog.Extensions;
using Serilog.Debugging;

namespace Serilog.Sinks.Graylog.Transport.Udp
{
    public sealed class UdpTransport : ITransport
    {
        private readonly IDataToChunkConverter chunkConverter;
        private readonly string hostnameOrAddress;
        private readonly int port;
        private readonly Func<UdpClient> factory;
        private readonly object locker;
        private UdpClient udpClient;

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
            this.factory = factory;
            locker = new object();
        }

        /// <inheritdoc />
        /// <summary>
        /// Sends the specified target.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="T:System.ArgumentException">message was too long</exception>
        /// <exception cref="T:System.ArgumentException">message was too long</exception>
        public Task SendAsync(string message)
        {
            // Initialize the client if it is new and create a message payload
            InitClient();
            var compressedMessage = message.Compress();
            var chunks = chunkConverter.ConvertToChunks(compressedMessage);

            // Udp client is not thread safe so take a lock
            lock (locker)
            {
                try
                {

                    var sent = 0;
                    foreach (var chunk in chunks)
                    {
                        udpClient.Send(chunk, chunk.Length, hostnameOrAddress, port);
                        SelfLog.WriteLine($"Sent UDP Gelf Chunk {++sent} of {chunks.Count}");
                    }
                }
                catch (Exception exc)
                {
                    SelfLog.WriteLine("Error sending UDP message chunks", exc);
                    Dispose();
                    throw;
                }
            }

            return Task.CompletedTask;
        }

        private void InitClient()
        {
            if (udpClient == null)
            {
                udpClient = factory?.Invoke() ?? new UdpClient();
            }
        }

        public void Dispose()
        {
            if (factory != null) return;
            lock (locker)
            {
                var disposable = udpClient as IDisposable;
                disposable?.Dispose();
                udpClient = null;
            }
        }
    }
}