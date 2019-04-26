using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Serilog.Sinks.Graylog.Core.Helpers;
using Serilog.Sinks.Graylog.Core.MessageBuilders;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.Graylog.Core.Transport.Http;
using Serilog.Sinks.Graylog.Core.Transport.Udp;
using SinkTransportType = Serilog.Sinks.Graylog.Core.Transport.TransportType;

namespace Serilog.Sinks.Graylog.Core
{
    public interface ISinkComponentsBuilder
    {
        ITransport MakeTransport();
        IGelfConverter MakeGelfConverter();
    }

    public class SinkComponentsBuilder : ISinkComponentsBuilder
    {
        private readonly GraylogSinkOptionsBase _options;
        private readonly Dictionary<BuilderType, Lazy<IMessageBuilder>> _builders;

        public SinkComponentsBuilder(GraylogSinkOptionsBase options)
        {
            _options = options;
            
            _builders = new Dictionary<BuilderType, Lazy<IMessageBuilder>>
            {
                [BuilderType.Exception] = new Lazy<IMessageBuilder>(() =>
                {
                    string hostName = Dns.GetHostName();
                    return new ExceptionMessageBuilder(hostName, _options);
                }),
                [BuilderType.Message] = new Lazy<IMessageBuilder>(() =>
                {
                    string hostName = Dns.GetHostName();
                    return new GelfMessageBuilder(hostName, _options);
                })
            };
        }

        public ITransport MakeTransport()
        {
            switch (_options.TransportType)
            {
                case SinkTransportType.Udp:

                    IDnsInfoProvider dns = new DnsWrapper();
                    IPAddress[] ipAddreses = Task.Run(() => dns.GetHostAddresses(_options.HostnameOrAddress)).Result;
                    IPAddress ipAddress = ipAddreses.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);

                    var ipEndpoint = new IPEndPoint(ipAddress ?? throw new InvalidOperationException(), _options.Port);


                    var chunkSettings = new ChunkSettings(_options.MessageGeneratorType, _options.MaxMessageSizeInUdp);
                    IDataToChunkConverter chunkConverter = new DataToChunkConverter(chunkSettings, new MessageIdGeneratorResolver());

                    var udpClient = new UdpTransportClient(ipEndpoint);
                    var udpTransport = new UdpTransport(udpClient, chunkConverter);
                    return udpTransport;

                case SinkTransportType.Http:
                    var httpClient = new HttpTransportClient($"{_options.HostnameOrAddress}:{_options.Port}/gelf");
                    var httpTransport = new HttpTransport(httpClient);
                    return httpTransport;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_options), _options.TransportType, null);
            }
        }

        public IGelfConverter MakeGelfConverter()
        {
            IGelfConverter converter = _options.GelfConverter ?? new GelfConverter(_builders);
            return converter;
        }
    }
}