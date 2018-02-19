using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Sinks.Graylog.Helpers;
using Serilog.Sinks.Graylog.Transport;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Graylog
{
    public class GraylogPeriodicBatchSink : PeriodicBatchingSink
    {
        private readonly IGelfConverter converter;
        private readonly LazyRetry<ITransport> transport;

        public GraylogPeriodicBatchSink(GraylogSinkOptions options)
            : base(options.BatchSizeLimit, options.Period)
        {
            transport = new LazyRetry<ITransport>(() => TransportFactory.FromOptions(options));
            converter = options.GelfConverter ?? GelfConverterFactory.FromOptions(options);
        }

        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (var @event in events)
            {
                var jObject = converter.GetGelfJson(@event);
                var json = jObject.ToString(Newtonsoft.Json.Formatting.None);
                await transport.Value.SendAsync(json).ConfigureAwait(false);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (transport.Created)
            {
                transport.Value.Dispose();
            }
        }
    }
}