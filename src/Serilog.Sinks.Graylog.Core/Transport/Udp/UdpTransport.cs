using Serilog.Sinks.Graylog.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Udp
{
    public class UdpTransport : ITransport
    {
        private readonly ITransportClient<byte[]> _transportClient;
        private readonly IDataToChunkConverter _chunkConverter;
        private readonly GraylogSinkOptionsBase _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTransport"/> class.
        /// </summary>
        /// <param name="transportClient">The transport client.</param>
        /// <param name="chunkConverter"></param>
        public UdpTransport(ITransportClient<byte[]> transportClient, IDataToChunkConverter chunkConverter, GraylogSinkOptionsBase options)
        {
            _transportClient = transportClient;
            _chunkConverter = chunkConverter;
            _options = options;
        }

        /// <summary>
        /// Sends the specified target.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentException">message was too long</exception>
        public Task Send(string message)
        {
            var payload = _options.UseGzip ? message.ToGzip() : message.ToByteArray();
            IList<byte[]> chunks = _chunkConverter.ConvertToChunks(payload);

            IEnumerable<Task> sendTasks = chunks.Select(c => _transportClient.Send(c));
            return Task.WhenAll(sendTasks.ToArray());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _transportClient.Dispose();
            }
        }
    }
}
