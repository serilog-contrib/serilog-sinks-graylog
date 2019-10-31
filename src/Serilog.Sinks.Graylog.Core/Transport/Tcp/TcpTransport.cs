using System;
using System.Collections.Generic;
using System.Threading;
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
            //Not support chunking and compressed payloads ='(
            var payload = System.Text.Encoding.UTF8.GetBytes(message);

            Array.Resize(ref payload, payload.Length + 1);
            payload[payload.Length - 1] = 0x00;

            return _tcpClient.Send(payload);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _tcpClient?.Dispose();
        }
    }
}