using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Transport.Udp
{
    /// <summary>
    /// Udp transport client
    /// </summary>
    /// <seealso cref="byte" />
    public sealed class UdpTransportClient : ITransportClient<byte[]>
    {
        private readonly IPEndPoint _target;
        private UdpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTransportClient"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public UdpTransportClient(IPEndPoint target)
        {
            _target = target;
            _client = new UdpClient(target);
        }

        /// <summary>
        /// Sends the specified payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public Task Send(byte[] payload)
        {
            return _client.SendAsync(payload, payload.Length, _target);
        }
    }
}