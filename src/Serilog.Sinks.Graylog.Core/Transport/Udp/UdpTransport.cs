using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Sinks.Graylog.Core.Extensions;

namespace Serilog.Sinks.Graylog.Core.Transport.Udp
{
    public class UdpTransport : ITransport
    {
        private readonly ITransportClient<byte[]> _transportClient;
        private readonly IDataToChunkConverter _chunkConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTransport"/> class.
        /// </summary>
        /// <param name="transportClient">The transport client.</param>
        /// <param name="chunkConverter"></param>
        public UdpTransport(ITransportClient<byte[]> transportClient, IDataToChunkConverter chunkConverter)
        {
            _transportClient = transportClient;
            _chunkConverter = chunkConverter;
        }


        /// <summary>
        /// Sends the specified target.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentException">message was too long</exception>
        public Task Send(string message)
        {
            byte[] compressedMessage = message.Compress();
            IList<byte[]> chunks = _chunkConverter.ConvertToChunks(compressedMessage);

            IEnumerable<Task> sendTasks = chunks.Select(c => _transportClient.Send(c));
            return Task.WhenAll(sendTasks.ToArray());
        }

        public void Dispose()
        {
            _transportClient?.Dispose();
        }
    }
}