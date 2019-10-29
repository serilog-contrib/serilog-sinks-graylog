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

        /// <inheritdoc />
        public TcpTransportClient(IPAddress address, int port)
        {
            _address = address;
            _port = port;
            _client = new TcpClient();
        }

        public Task Connect()
        {
            return _client.ConnectAsync(_address, _port);
        }

        /// <inheritdoc />
        public async Task Send(byte[] payload)
        {
            if (!_client.Connected)
            {
                await _client.ConnectAsync(_address, _port).ConfigureAwait(false);
            }

            using (var stream = _client.GetStream())
            {
                await stream.WriteAsync(payload, 0, payload.Length).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            #if NETFRAMEWORK
            _client.Close();
            #else
            _client.Dispose();
            #endif
        }
    }
}