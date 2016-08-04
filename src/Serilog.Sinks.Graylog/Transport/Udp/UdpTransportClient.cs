using System.Net;
using System.Net.Sockets;

namespace Serilog.Sinks.Graylog.Transport.Udp
{
    /// <summary>
    /// Udp transport client
    /// </summary>
    /// <seealso cref="byte" />
    public sealed class UdpTransportClient : ITransportClient<byte[]>
    {
        private readonly IPEndPoint _target;

        public UdpTransportClient(IPEndPoint target)
        {
            _target = target;
        }

        /// <summary>
        /// Sends the specified payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="SocketException">Произошла ошибка при получении доступа к сокету.Дополнительные сведения см. в разделе "Примечания".</exception>
        public void Send(byte[] payload)
        {
            using (var udpClient = new UdpClient())
            {
                udpClient.Send(payload, payload.Length, _target);
            }
        }
    }
}