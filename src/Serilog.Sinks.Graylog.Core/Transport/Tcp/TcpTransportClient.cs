using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Tcp
{
    public class TcpTransportClient : ITransportClient<byte[]>
    {
        private readonly IPAddress _address;
        private readonly TcpClient _client;
        private readonly int _port;
        private NetworkStream _stream;

        /// <inheritdoc />
        public TcpTransportClient(IPAddress address, int port)
        {
            _address = address;
            _port = port;
            _client = new TcpClient();
        }

        public async Task Connect()
        {
            await _client.ConnectAsync(_address, _port).ConfigureAwait(false);
            _stream = _client.GetStream();
        }

        /// <inheritdoc />
        public async Task Send(byte[] payload)
        {
            await _stream.WriteAsync(payload, 0, payload.Length).ConfigureAwait(false);
            await _stream.FlushAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            #if NETFRAMEWORK
            _client.Close();
            #else
            _client?.Dispose();
            
            #endif
            _stream?.Dispose();
        }
    }
}