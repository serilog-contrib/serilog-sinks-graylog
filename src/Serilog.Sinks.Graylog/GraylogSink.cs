using System.Linq;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Graylog.Transport;
using Serilog.Sinks.Graylog.Transport.Udp;


namespace Serilog.Sinks.Graylog
{
    public class GraylogSink : ILogEventSink
    {
        private readonly GraylogSinkOptions _options;
        private readonly IGelfConverter _converter;
        private readonly ITransport _transport;

        public GraylogSink(GraylogSinkOptions options)
        {
            _options = options;

            IDnsInfoProvider dns = new DnsWrapper();
            IPAddress ipAdress = dns.GetHostAddresses(options.HostnameOrAdress)
                                     .FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);

            var ipEndpoint = new IPEndPoint(ipAdress, options.Port);

            IDataToChunkConverter chunkConverter = new DataToChunkConverter(dns, new ChunkSettings
            {
                MessageIdGeneratorType = options.MessageGeneratorType
            });

            var client = new UdpTransportClient(ipEndpoint);
            _transport = _options.Transport ?? new UdpTransport(client, chunkConverter);
            _converter = _options.GelfConverter ?? new GelfConverter();
        }

        public void Emit(LogEvent logEvent)
        {
            JObject json = _converter.GetGelfJson(logEvent, _options);
            _transport.Send(json.ToString(Newtonsoft.Json.Formatting.None));
        }
    }
}