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

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTransportClient"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public UdpTransportClient(IPEndPoint target)
        {
            _target = target;
        }

        /// <summary>
        /// Sends the specified payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public async Task Send(byte[] payload)
        {
            using (var udpClient = new UdpClient())
            {
                await udpClient.SendAsync(payload, payload.Length, _target);
            }
        }
    }
}