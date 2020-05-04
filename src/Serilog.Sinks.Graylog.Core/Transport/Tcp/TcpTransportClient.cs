using Serilog.Sinks.Graylog.Core.Extensions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Tcp
{
    public class TcpTransportClient : ITransportClient<byte[]>
    {
        private readonly IPAddress _address;
        private readonly int _port;
        private TcpClient _client;
        private NetworkStream _stream;

        /// <inheritdoc />
        public TcpTransportClient(IPAddress address, int port)
        {
            _address = address;
            _port = port;
        }

        /// <inheritdoc />
        public async Task Send(byte[] payload)
        {
            await CheckSocketConnection();

            await _stream.WriteAsync(payload, 0, payload.Length).ConfigureAwait(false);
            await _stream.FlushAsync().ConfigureAwait(false);
        }

        private async Task CheckSocketConnection()
        {
            if (_client != null)
            {
                if (_client.Connected)
                    return;
                else
                    CloseClient();
            }

            _client = new TcpClient();
            await Connect();
        }

        private async Task Connect()
        {
            await _client.ConnectAsync(_address, _port).ConfigureAwait(false);
            _stream = _client.GetStream();
        }

        private void CloseClient()
        {
#if NETFRAMEWORK
            _client?.Close();
#else
            _client?.Dispose();
#endif
            _stream?.Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            CloseClient();
        }

    }
}