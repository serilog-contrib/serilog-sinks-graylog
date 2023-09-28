using Serilog.Sinks.Graylog.Core.Helpers;
using Serilog.Sinks.Graylog.Core.MessageBuilders;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.Graylog.Core.Transport.Http;
using Serilog.Sinks.Graylog.Core.Transport.Tcp;
using Serilog.Sinks.Graylog.Core.Transport.Udp;
using System;
using System.Collections.Generic;
using System.Net;
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
                [BuilderType.Exception] = new(() =>
                {
                    string hostName = Dns.GetHostName();
                    return new ExceptionMessageBuilder(hostName, _options);
                }),
                [BuilderType.Message] = new(() =>
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
                    var chunkSettings = new ChunkSettings(_options.MessageGeneratorType, _options.MaxMessageSizeInUdp);
                    IDataToChunkConverter chunkConverter = new DataToChunkConverter(chunkSettings, new MessageIdGeneratorResolver());

                    var udpClient = new UdpTransportClient(_options, new DnsWrapper());
                    var udpTransport = new UdpTransport(udpClient, chunkConverter, _options);

                    return udpTransport;
                case SinkTransportType.Http:
                    var httpClient = new HttpTransportClient(_options);

                    return new HttpTransport(httpClient);
                case SinkTransportType.Tcp:
                    var tcpClient = new TcpTransportClient(_options, new DnsWrapper());

                    return new TcpTransport(tcpClient);
                case SinkTransportType.Custom:
                    if (_options.TransportFactory == null)
                    {
                        throw new InvalidOperationException("The TransportFactory value must have a value.");
                    }

                    return _options.TransportFactory();
                default:
                    throw new ArgumentOutOfRangeException(nameof(_options), _options.TransportType, null);
            }
        }

        public IGelfConverter MakeGelfConverter() => _options.GelfConverter ?? new GelfConverter(_builders);
    }
}
