using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Udp
{
    /// <summary>
    /// Udp transport client
    /// </summary>
    /// <seealso cref="byte" />
    public sealed class UdpTransportClient : ITransportClient<byte[]>
    {
        private IPEndPoint? _ipEndPoint;

        private readonly GraylogSinkOptionsBase _options;
        private readonly UdpClient _client;

        public UdpTransportClient(GraylogSinkOptionsBase options)
        {
            _options = options;

            _client = new UdpClient();
        }

        private static async Task<IPAddress?> GetIpAddress(string? hostnameOrAddress)
        {
            if (string.IsNullOrEmpty(hostnameOrAddress))
            {
                return null;
            }

            IDnsInfoProvider dns = new DnsWrapper();
            IPAddress[] ipAddresses = await dns.GetHostAddresses(hostnameOrAddress!).ConfigureAwait(false);

            return ipAddresses.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
        }

        private async Task EnsureTarget()
        {
            if (_ipEndPoint == null)
            {
                var ipAddress = await GetIpAddress(_options.HostnameOrAddress).ConfigureAwait(false) ?? throw new InvalidOperationException("IP address could not be resolved.");

                _ipEndPoint = new IPEndPoint(ipAddress, _options.Port.GetValueOrDefault(12201));
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
