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
        private readonly Lazy<IGelfConverter> _converter;
        private readonly Lazy<ITransport> _transport;

        
        public GraylogSink(GraylogSinkOptions options)
        {
            ISinkComponentsBuilder sinkComponentsBuilder = new SinkComponentsBuilder(options);
            _transport = new Lazy<ITransport>(() => sinkComponentsBuilder.MakeTransport());
            _converter = new Lazy<IGelfConverter>(() => sinkComponentsBuilder.MakeGelfConverter());
        }

        public void Emit(LogEvent logEvent)
        {
            try
            {
                Task emitTask = EmitAsync(logEvent);
                emitTask.RunSynchronously(TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
            }
        }

        private Task EmitAsync(LogEvent logEvent)
        {
            JObject json = _converter.Value.GetGelfJson(logEvent);
            string payload = json.ToString(Newtonsoft.Json.Formatting.None);
            return _transport.Value.Send(payload);
        }
    }
}