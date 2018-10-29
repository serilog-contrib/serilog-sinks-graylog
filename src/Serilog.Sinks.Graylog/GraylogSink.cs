using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Transport;

namespace Serilog.Sinks.Graylog
{
    public class GraylogSink : ILogEventSink
    {
        private readonly IGelfConverter _converter;
        private readonly ITransport _transport;

        public GraylogSink(GraylogSinkOptions options)
        {
            ISinkComponentsBuilder sinkComponentsBuilder = new SinkComponentsBuilder(options);
            _transport = sinkComponentsBuilder.MakeTransport();
            _converter = sinkComponentsBuilder.MakeGelfConverter();
        }

        public void Emit(LogEvent logEvent)
        {
            try
            {
                Task.Run(() => EmitAsync(logEvent)).GetAwaiter().GetResult();
            }
            catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
            }
        }

        private Task EmitAsync(LogEvent logEvent)
        {
            JObject json = _converter.GetGelfJson(logEvent);
            string payload = json.ToString(Newtonsoft.Json.Formatting.None);
            return _transport.Send(payload);
        }
    }
}