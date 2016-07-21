using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Graylog.Transport;
using Serilog.Sinks.PeriodicBatching;


namespace Serilog.Sinks.Graylog
{
    public class GraylogSink : ILogEventSink, IDisposable
    {
        private readonly DnsBase _dns;
        private readonly GraylogSinkOptions _options;
        private readonly ITransport _transport;
        
        private readonly GelfConverter _converter;
        private readonly IPEndPoint _ipEndpoint;

        public GraylogSink(GraylogSinkOptions options)
        {
            _dns = new DnsWrapper();
            _options = options;
            
            
            IPAddress ipAdress = _dns.GetHostAddresses(options.HostnameOrAdress)
                                     .FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
            _ipEndpoint = new IPEndPoint(ipAdress, options.Port);

            var client = new UdpTransportClient(_ipEndpoint);
            _transport = new UDPTransport(client);

            _converter = new GelfConverter();
        }

        public void Emit(LogEvent logEvent)
        {
            JObject json = _converter.GetGelfJson(logEvent, _options.Facility);
            _transport.Send(json.ToString(Newtonsoft.Json.Formatting.None));

        }

        public void Dispose()
        {
            
        }
    }
}