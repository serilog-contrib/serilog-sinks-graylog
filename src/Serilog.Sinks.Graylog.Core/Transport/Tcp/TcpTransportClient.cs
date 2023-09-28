using Serilog.Debugging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Tcp
{
    public class TcpTransportClient : ITransportClient<byte[]>
    {
        private const int DefaultPort = 12201;

        private Stream? _stream;

        private readonly GraylogSinkOptionsBase _options;
        private readonly IDnsInfoProvider _dnsInfoProvider;
        private readonly TcpClient _client;

        /// <inheritdoc />
        public TcpTransportClient(GraylogSinkOptionsBase options, IDnsInfoProvider dnsInfoProvider)
        {
            _options = options;
            _dnsInfoProvider = dnsInfoProvider;

            _client = new TcpClient();
        }

        /// <inheritdoc />
        public async Task Send(byte[] payload)
        {
            await EnsureConnection().ConfigureAwait(false);

#if NETSTANDARD2_0
            await _stream!.WriteAsync(payload, 0, payload.Length).ConfigureAwait(false);
#else
            await _stream!.WriteAsync(payload).ConfigureAwait(false);
#endif

            await _stream.FlushAsync().ConfigureAwait(false);
        }

        private async Task EnsureConnection()
        {
            if (!_client.Connected)
            {
                await Connect().ConfigureAwait(false);
            }
        }

        private async Task Connect()
        {
            IPAddress? _address = await _dnsInfoProvider.GetIpAddress(_options.HostnameOrAddress!);
            if (_address == default)
            {
                SelfLog.WriteLine("IP address could not be resolved.");
                return;
            }

            int port = _options.Port.GetValueOrDefault(DefaultPort);
            string? sslHost = _options.UseSsl ? _options.HostnameOrAddress : null;

            await _client.ConnectAsync(_address, port).ConfigureAwait(false);

            _stream = _client.GetStream();

            if (!string.IsNullOrWhiteSpace(sslHost))
            {
                var _sslStream = new SslStream(_stream, false);

                await _sslStream.AuthenticateAsClientAsync(sslHost).ConfigureAwait(false);

                if (_sslStream.RemoteCertificate != null)
                {
                    SelfLog.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                        _sslStream.RemoteCertificate.Subject,
                        _sslStream.RemoteCertificate.GetEffectiveDateString(),
                        _sslStream.RemoteCertificate.GetExpirationDateString());

                    _stream = _sslStream;
                } else
                {
                    SelfLog.WriteLine("Remote certificate is null.");
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream?.Dispose();
                _client.Dispose();
            }
        }
    }
}
