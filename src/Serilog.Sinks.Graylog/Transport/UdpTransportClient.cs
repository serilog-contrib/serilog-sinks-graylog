using System.Net;
using System.Net.Sockets;

namespace Serilog.Sinks.Graylog.Transport
{
    /// <summary>
    /// Udp transport client
    /// </summary>
    /// <seealso cref="Serilog.Sinks.Graylog.Transport.ITransportClient" />
    public class UdpTransportClient : ITransportClient
    {
        /// <summary>
        /// Sends the specified payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="bytesLength">Length of the bytes.</param>
        /// <param name="target">The target.</param>
        /// <exception cref="SocketException">Произошла ошибка при получении доступа к сокету.Дополнительные сведения см. в разделе "Примечания".</exception>
        public void Send(byte[] payload, int bytesLength, IPEndPoint target)
        {
            using (var udpClient = new UdpClient())
            {
                udpClient.Send(payload, bytesLength, target);
            }
        }
    }
}