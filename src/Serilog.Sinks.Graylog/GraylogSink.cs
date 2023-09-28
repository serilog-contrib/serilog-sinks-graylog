using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Transport;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog
{
    public sealed class GraylogSink : ILogEventSink, IDisposable
    {
        private readonly Lazy<IGelfConverter> _converter;
        private readonly Lazy<ITransport> _transport;
        private readonly JsonSerializerOptions _options;

        public GraylogSink(GraylogSinkOptions options)
        {
            ISinkComponentsBuilder sinkComponentsBuilder = new SinkComponentsBuilder(options);

            var jsonSerializerOptions = options.JsonSerializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.General);
            _options = new JsonSerializerOptions(jsonSerializerOptions);

            _transport = new Lazy<ITransport>(sinkComponentsBuilder.MakeTransport);
            _converter = new Lazy<IGelfConverter>(() => sinkComponentsBuilder.MakeGelfConverter());
        }

        //should work
        public async void Emit(LogEvent logEvent)
        {
            try
            {
                await EmitAsync(logEvent).ConfigureAwait(false);
            } catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
            }
        }

        private Task EmitAsync(LogEvent logEvent)
        {
            var json = _converter.Value.GetGelfJson(logEvent);
            var payload = json.ToJsonString(_options);

            return _transport.Value.Send(payload);
        }

        public void Dispose()
        {
            _transport.Value.Dispose();
        }
    }
}
