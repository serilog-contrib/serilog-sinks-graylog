using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Transport;

namespace Serilog.Sinks.Graylog
{
    public class GraylogSink : ILogEventSink, IDisposable
    {
        private readonly Lazy<IGelfConverter> _converter;
        private readonly Lazy<ITransport> _transport;
        private readonly JsonSerializer _serializer;


        public GraylogSink(GraylogSinkOptions options)
        {
            ISinkComponentsBuilder sinkComponentsBuilder = new SinkComponentsBuilder(options);
            _serializer = JsonSerializer.Create(options.SerializerSettings);
            _transport = new Lazy<ITransport>(() => sinkComponentsBuilder.MakeTransport());
            _converter = new Lazy<IGelfConverter>(() => sinkComponentsBuilder.MakeGelfConverter());
        }
        
        public void Dispose()
        {
            _transport.Value.Dispose();
        }

        public void Emit(LogEvent logEvent)
        {
            try
            {
                EmitAsync(logEvent).ConfigureAwait(false)
                                   .GetAwaiter()
                                   .GetResult();
            }
            catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
            }
        }

        private Task EmitAsync(LogEvent logEvent)
        {
            JObject json = _converter.Value.GetGelfJson(logEvent);

            using (var textWriter = new StringWriter())
            {
                _serializer.Serialize(textWriter, json);
                var payload = textWriter.ToString();
                return _transport.Value.Send(payload);
            }
            //string payload = json.ToString(settings.Formatting, settings.Converters.ToArray());
            //string payload = json.ToString(settings.Formatting, settings.Converters.ToArray());
            //return _transport.Value.Send(payload);
        }
    }
}
