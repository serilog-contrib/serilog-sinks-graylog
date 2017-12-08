using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
        private readonly ITransport _transport;

        public GraylogSink(GraylogSinkOptions options)
        {
            _transport = MakeTransport(options);

            string hostName = Dns.GetHostName();

            IDictionary<BuilderType, Lazy<IMessageBuilder>> builders = new Dictionary<BuilderType, Lazy<IMessageBuilder>>
            {
                [BuilderType.Exception] = new Lazy<IMessageBuilder>(() => new ExceptionMessageBuilder(hostName, options)),
                [BuilderType.Message] = new Lazy<IMessageBuilder>(() => new GelfMessageBuilder(hostName, options))
            };
              
            _converter = options.GelfConverter ?? new GelfConverter(builders);
        }

        private ITransport MakeTransport(GraylogSinkOptions options)
        {
            switch (options.TransportType)
            {
                case SerilogTransportType.Udp:

                    IDnsInfoProvider dns = new DnsWrapper();
                    IPAddress[] ipAddreses = Task.Run(() => dns.GetHostAddresses(options.HostnameOrAddress)).Result;
                    IPAddress ipAddress = ipAddreses.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
                    var ipEndpoint = new IPEndPoint(ipAddress, options.Port);

                    IDataToChunkConverter chunkConverter = new DataToChunkConverter(new ChunkSettings
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

                JObject json = _converter.GetGelfJson(logEvent);

                Task.Run(async () => await _transport.Send(json.ToString(Newtonsoft.Json.Formatting.None)).ConfigureAwait(false)).GetAwaiter().GetResult();
            }
            catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
            }
        }
    }
}