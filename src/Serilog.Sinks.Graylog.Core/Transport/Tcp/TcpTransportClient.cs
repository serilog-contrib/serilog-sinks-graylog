using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Tcp
{
    public class TcpTransportClient : ITransportClient<byte[]>
    {
        private readonly IPAddress _address;
        private readonly int _port;
        private readonly string _sslHost;
        private TcpClient _client;
        private Stream _stream;

        /// <inheritdoc />
        public TcpTransportClient(IPAddress address, int port, string sslHost)
        {
            _address = address;
            _port = port;
            _sslHost = sslHost;
        }

        /// <inheritdoc />
        public async Task Send(byte[] payload)
        {
            await CheckSocketConnection().ConfigureAwait(false);

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
            await Connect().ConfigureAwait(false);
        }

        private async Task Connect()
        {
            await _client.ConnectAsync(_address, _port).ConfigureAwait(false);
            _stream = _client.GetStream();

            if (!string.IsNullOrWhiteSpace(_sslHost))
            {
                var _sslStream = new SslStream(_stream, false);

                await _sslStream.AuthenticateAsClientAsync(_sslHost).ConfigureAwait(false);

                X509Certificate remoteCertificate = _sslStream.RemoteCertificate;
                if (_sslStream.RemoteCertificate != null)
                {
                    Debug.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                        remoteCertificate.Subject,
                        remoteCertificate.GetEffectiveDateString(),
                        remoteCertificate.GetExpirationDateString());

                    _stream = _sslStream;
                }
                else
                {
                    Debug.Fail("Remote certificate is null.");
                }
            }
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
