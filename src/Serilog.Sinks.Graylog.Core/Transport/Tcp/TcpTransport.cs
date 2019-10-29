using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Tcp
{
    public class TcpTransport : ITransport
    {
        private readonly ITransportClient<byte[]> _tcpClient;
        /// <summary>
        /// The default gelf magic line ending
        /// </summary>
        /// <seealso cref="https://docs.graylog.org/en/3.1/pages/gelf.html#gelf-via-tcp"/>
        private const string DefaultGelfMagicLineEnding = @"\0";

        /// <inheritdoc />
        public TcpTransport(ITransportClient<byte[]> tcpClient)
        {
            _tcpClient = tcpClient;
        }

        /// <inheritdoc />
        public Task Send(string message)
        {
            var payloadMessage = $"{message}{DefaultGelfMagicLineEnding}";

            //Not support chunking and compressed payloads ='(
            var payload = System.Text.Encoding.UTF8.GetBytes(payloadMessage);
            return _tcpClient.Send(payload);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _tcpClient?.Dispose();
        }
    }
}