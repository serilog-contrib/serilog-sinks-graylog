using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Udp
{
    using Debugging;

    /// <summary>
    /// Udp transport client
    /// </summary>
    /// <seealso cref="byte" />
    public sealed class UdpTransportClient : ITransportClient<byte[]>
    {
        private IPEndPoint? _ipEndPoint;

        private readonly GraylogSinkOptionsBase _options;
        private readonly IDnsInfoProvider _dnsInfoProvider;
        private readonly UdpClient _client;

        public UdpTransportClient(GraylogSinkOptionsBase options, IDnsInfoProvider dnsInfoProvider)
        {
            _options = options;
            _dnsInfoProvider = dnsInfoProvider;

            _client = new UdpClient();
        }

        private async Task EnsureTarget()
        {
            if (_ipEndPoint == null)
            {
                var ipAddress = await _dnsInfoProvider.GetIpAddress(_options.HostnameOrAddress!).ConfigureAwait(false);
                if (ipAddress == default)
                {
                    SelfLog.WriteLine("IP address could not be resolved.");
                    return;
                }
                _ipEndPoint = new IPEndPoint(ipAddress, _options.Port.GetValueOrDefault());
            }
        }

        /// <summary>
        /// Sends the specified payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public async Task Send(byte[] payload)
        {
            await EnsureTarget().ConfigureAwait(false);

            await _client.SendAsync(payload, payload.Length, _ipEndPoint).ConfigureAwait(false);
        }

        public void Dispose() => _client.Dispose();
    }
}
