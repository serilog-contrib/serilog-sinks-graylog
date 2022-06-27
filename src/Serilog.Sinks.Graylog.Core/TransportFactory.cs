﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Serilog.Sinks.Graylog.Core.Helpers;
using Serilog.Sinks.Graylog.Core.MessageBuilders;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.Graylog.Core.Transport.Http;
using Serilog.Sinks.Graylog.Core.Transport.Tcp;
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
        private const string _emptyHttpUriPath = "/";
        private const string _defaultHttpUriPath = "/gelf";
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
                {
                    var ipAddress = Task.Run(() => GetIpAddress(_options.HostnameOrAddress)).GetAwaiter().GetResult();
                    var ipEndpoint = new IPEndPoint(ipAddress ?? throw new InvalidOperationException(), _options.Port.GetValueOrDefault(12201));


                    var chunkSettings = new ChunkSettings(_options.MessageGeneratorType, _options.MaxMessageSizeInUdp);
                    IDataToChunkConverter chunkConverter =
                        new DataToChunkConverter(chunkSettings, new MessageIdGeneratorResolver());

                    var udpClient = new UdpTransportClient(ipEndpoint);
                    var udpTransport = new UdpTransport(udpClient, chunkConverter);
                    return udpTransport;
                }
                case SinkTransportType.Http: 
                {
                    var builder = GetUriBuilder(_options.HostnameOrAddress, _options.UseSsl);
                    var httpClient = new HttpTransportClient(builder.Uri.ToString(), new HttpBasicAuthenticationGenerator(_options.UsernameInHttp, _options.PasswordInHttp).Generate());

                    var httpTransport = new HttpTransport(httpClient);
                    return httpTransport;
                }
                case SinkTransportType.Tcp:
                {
                    var ipAddress = Task.Run(() => GetIpAddress(_options.HostnameOrAddress)).GetAwaiter().GetResult();
                    var tcpClient = new TcpTransportClient(ipAddress, _options.Port.GetValueOrDefault(12201), _options.UseSsl ? _options.HostnameOrAddress : null);
                    var transport = new TcpTransport(tcpClient);
                    return transport;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(_options), _options.TransportType, null);
            }
        }

        private async Task<IPAddress> GetIpAddress(string hostnameOrAddress)
        {
            IDnsInfoProvider dns = new DnsWrapper();
            IPAddress[] ipAddreses = await dns.GetHostAddresses(hostnameOrAddress).ConfigureAwait(false);
            IPAddress ipAddress = ipAddreses.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
            return ipAddress;
        }

        private UriBuilder GetUriBuilder(string hostnameOrAddress, bool useSsl)
        {
            var builder = new UriBuilder(hostnameOrAddress)
            {
                Port = _options.Port.GetValueOrDefault(443),
                Scheme = useSsl ? "https" : "http"
            };

            if (builder.Path == _emptyHttpUriPath)
                builder.Path = _defaultHttpUriPath;

            return builder;
        }

        public IGelfConverter MakeGelfConverter()
        {
            IGelfConverter converter = _options.GelfConverter ?? new GelfConverter(_builders);
            return converter;
        }
    }
}