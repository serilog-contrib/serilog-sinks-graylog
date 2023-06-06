using System;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Tcp
{
    public class TcpTransport : ITransport
    {
        private readonly ITransportClient<byte[]> _tcpClient;

        /// <inheritdoc />
        public TcpTransport(ITransportClient<byte[]> tcpClient)
        {
            _tcpClient = tcpClient;
        }

        /// <inheritdoc />
        public Task Send(string message)
        {
            var payload = new byte[message.Length + 1];
            System.Text.Encoding.UTF8.GetBytes(message.AsSpan(), payload.AsSpan());
            payload[^1] = 0x00;

            return _tcpClient.Send(payload);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _tcpClient?.Dispose();
        }
    }
}