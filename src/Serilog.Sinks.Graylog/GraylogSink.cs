using System;
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