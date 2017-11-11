using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Graylog.Helpers;
using Serilog.Sinks.Graylog.MessageBuilders;
using Serilog.Sinks.Graylog.Transport;
using Serilog.Sinks.Graylog.Transport.Http;
using Serilog.Sinks.Graylog.Transport.Udp;
using SerilogTransportType = Serilog.Sinks.Graylog.Transport.TransportType;


namespace Serilog.Sinks.Graylog
{
    public class GraylogSink : ILogEventSink
    {
        private readonly IGelfConverter _converter;
        private readonly Lazy<ITransport> _transport;
        private readonly GraylogSinkOptions options;

        public GraylogSink(GraylogSinkOptions graylogSinkOptions)
        {
            options = graylogSinkOptions ?? new GraylogSinkOptions();
            _transport = new Lazy<ITransport>(() => MakeTransport(options));
            _converter = options.GelfConverter ?? CreateDefaultConverter(options);
        }

        private static IGelfConverter CreateDefaultConverter(GraylogSinkOptions options)
        {
            var hostName = Dns.GetHostName();
            var builders = new Dictionary<BuilderType, Lazy<IMessageBuilder>>
            {
                [BuilderType.Exception] =
                new Lazy<IMessageBuilder>(() => new ExceptionMessageBuilder(hostName, options)),
                [BuilderType.Message] =
                new Lazy<IMessageBuilder>(() => new GelfMessageBuilder(hostName, options))
            };

            return new GelfConverter(builders);
        }

        private static ITransport MakeTransport(GraylogSinkOptions options)
        {
            switch (options.TransportType)
            {
                case SerilogTransportType.Udp:
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
                case SerilogTransportType.Http:
                    var httpClient = new HttpTransportClient($"{options.HostnameOrAddress}:{options.Port}/gelf");
                    var httpTransport = new HttpTransport(httpClient);
                    return httpTransport;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options), options.TransportType, null);
            }
        }

        public void Emit(LogEvent logEvent)
        {
            try
            {
                var jObject = _converter.GetGelfJson(logEvent);
                var json = jObject.ToString(Newtonsoft.Json.Formatting.None);
                Task.Run(async () => await _transport.Value.Send(json).ConfigureAwait(false))
                    .GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                SelfLog.WriteLine("Exception while emitting from {0}: {1}", this, e);
                if (options.ThrowInternalErrors) throw;
            }
        }
    }
}