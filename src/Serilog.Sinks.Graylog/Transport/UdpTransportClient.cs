using System.Net;
using System.Net.Sockets;

namespace Serilog.Sinks.Graylog.Transport
{
    /// <summary>
    /// Udp transport client
    /// </summary>
    /// <seealso cref="Serilog.Sinks.Graylog.Transport.ITransportClient" />
    public sealed class UdpTransportClient : ITransportClient
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
        /// <param name="bytesLength">Length of the bytes.</param>
        /// <exception cref="SocketException">Произошла ошибка при получении доступа к сокету.Дополнительные сведения см. в разделе "Примечания".</exception>
        public void Send(byte[] payload, int bytesLength)
        {
            using (var udpClient = new UdpClient())
            {
                udpClient.Send(payload, bytesLength, _target);
            }
        }
    }
}