using System.Net;

namespace Serilog.Sinks.Graylog.Transport
{
    /// <summary>
    /// The Transport client interface
    /// </summary>
    public interface ITransportClient
    {
        /// <summary>
        /// Sends the specified payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="bytesLength">Length of the bytes.</param>
        /// <param name="target">The target.</param>
        void Send(byte[] payload, int bytesLength, IPEndPoint target);
    }
}