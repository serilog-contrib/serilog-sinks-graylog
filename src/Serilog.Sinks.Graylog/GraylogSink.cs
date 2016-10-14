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

            IDataToChunkConverter chunkConverter = new DataToChunkConverter(new ChunkSettings
            {
                MessageIdGeneratorType = options.MessageGeneratorType
            }, new MessageIdGeneratorResolver());

            var client = new UdpTransportClient(ipEndpoint);
            _transport = options.Transport ?? new UdpTransport(client, chunkConverter);

            string hostName = Dns.GetHostName();

            IDictionary<BuilderType, IMessageBuilder> builders = new Dictionary<BuilderType, IMessageBuilder>
            {
                [BuilderType.Exception] = new ExceptionMessageBuilder(hostName, options),
                [BuilderType.Message] = new GelfMessageBuilder(hostName, options)
            };
              
            _converter = options.GelfConverter ?? new GelfConverter(builders);
        }

        public void Emit(LogEvent logEvent)
        {
            JObject json = _converter.GetGelfJson(logEvent);
            _transport.Send(json.ToString(Newtonsoft.Json.Formatting.None));
        }
    }
}