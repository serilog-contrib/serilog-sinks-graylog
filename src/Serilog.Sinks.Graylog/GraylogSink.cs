using System;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Graylog.Transport;
using Serilog.Sinks.Graylog.Helpers;

namespace Serilog.Sinks.Graylog
{
    public class GraylogSink : ILogEventSink
    {
        private readonly IGelfConverter _converter;
        private readonly LazyRetry<ITransport> _transport;
        private readonly GraylogSinkOptions options;

        public GraylogSink(GraylogSinkOptions graylogSinkOptions, Func<ITransport> transportFactory = null)
        {
            options = graylogSinkOptions ?? new GraylogSinkOptions();
            _transport = new LazyRetry<ITransport>(transportFactory ?? TransportFactory.FromOptions(options));
            _converter = options.GelfConverter ?? GelfConverterFactory.FromOptions(options).Invoke();
        }

        public void Emit(LogEvent logEvent)
        {
            try
            {
                var jObject = _converter.GetGelfJson(logEvent);
                var json = jObject.ToString(Newtonsoft.Json.Formatting.None);
                Task.Run(async () => await _transport.Value.Send(json).ConfigureAwait(false))
                    .GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                SelfLog.WriteLine("Exception while emitting from {0}: {1}", this, e);
                if (options.ThrowOnSendError) throw;
            }
        }
    }
}