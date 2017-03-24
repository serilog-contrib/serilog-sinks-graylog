using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Graylog.Helpers;
using Serilog.Sinks.Graylog.MessageBuilders;
using Serilog.Sinks.Graylog.Transport;
using Serilog.Sinks.Graylog.Transport.Udp;
using SerilogTransportType = Serilog.Sinks.Graylog.Transport.TransportType;


namespace Serilog.Sinks.Graylog
{
    public class GraylogSink : ILogEventSink
    {
        private readonly IGelfConverter _converter;
        private readonly ITransport _transport;

        public GraylogSink(GraylogSinkOptions options)
        {
            IDnsInfoProvider dns = new DnsWrapper();
            IPAddress[] ipAddreses = Task.Run(() => dns.GetHostAddresses(options.HostnameOrAdress)).Result;
            IPAddress ipAdress = ipAddreses.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
            var ipEndpoint = new IPEndPoint(ipAdress, options.Port);

            _transport = MakeTransport(options, ipEndpoint);
            

            string hostName = Dns.GetHostName();

            IDictionary<BuilderType, Lazy<IMessageBuilder>> builders = new Dictionary<BuilderType, Lazy<IMessageBuilder>>
            {
                [BuilderType.Exception] = new Lazy<IMessageBuilder>(() => new ExceptionMessageBuilder(hostName, options)),
                [BuilderType.Message] = new Lazy<IMessageBuilder>(() => new GelfMessageBuilder(hostName, options))
            };
              
            _converter = options.GelfConverter ?? new GelfConverter(builders);
        }

        private ITransport MakeTransport(GraylogSinkOptions options, IPEndPoint ipEndpoint)
        {
            switch (options.TransportType)
            {
                case SerilogTransportType.Udp:
                    IDataToChunkConverter chunkConverter = new DataToChunkConverter(new ChunkSettings
                    {
                        MessageIdGeneratorType = options.MessageGeneratorType
                    }, new MessageIdGeneratorResolver());

                    var client = new UdpTransportClient(ipEndpoint);
                    var transport = new UdpTransport(client, chunkConverter);
                    return transport;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options), options.TransportType, null);
            }
            
        }

        public void Emit(LogEvent logEvent)
        {
            JObject json = _converter.GetGelfJson(logEvent);

            Task.Factory.StartNew(() => _transport.Send(json.ToString(Newtonsoft.Json.Formatting.None))).GetAwaiter().GetResult();
        }
    }
}