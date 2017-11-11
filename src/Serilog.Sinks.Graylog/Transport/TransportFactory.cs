using Serilog.Sinks.Graylog.Helpers;
using Serilog.Sinks.Graylog.Transport.Http;
using Serilog.Sinks.Graylog.Transport.Udp;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Transport
{
    public static class TransportFactory
    {
        public static Func<ITransport> FromOptions(GraylogSinkOptions options)
        {
            switch (options.TransportType)
            {
                case TransportType.Udp:
                    return CreateUdpFactory(options);
                case TransportType.Http:
                    return CreateHttpFactory(options);
                default:
                    throw new ArgumentOutOfRangeException(nameof(options), options.TransportType, null);
            }
        }

        private static Func<ITransport> CreateHttpFactory(GraylogSinkOptions options) => () =>
        {
            var httpClient = new HttpTransportClient($"{options.HostnameOrAddress}:{options.Port}/gelf");
            var httpTransport = new HttpTransport(httpClient);
            return httpTransport;
        };

        private static Func<ITransport> CreateUdpFactory(GraylogSinkOptions options) => () =>
        {
            var dns = new DnsWrapper();
            var ipAddreses = Task.Run(() => dns.GetHostAddresses(options.HostnameOrAddress)).Result;
            var ipAddress = ipAddreses.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
            var ipEndpoint = new IPEndPoint(ipAddress, options.Port);

            var chunkConverter = new DataToChunkConverter(new ChunkSettings
            {
                MessageIdGeneratorType = options.MessageGeneratorType
            }, new MessageIdGeneratorResolver());

            var udpClient = new UdpTransportClient(ipEndpoint);
            var udpTransport = new UdpTransport(udpClient, chunkConverter);
            return udpTransport;
        };
    }

}
