using Serilog.Sinks.Graylog.Helpers;
using Serilog.Sinks.Graylog.Transport;

namespace Serilog.Sinks.Graylog
{
    public class GraylogSink : ILogEventSink
    {
        private readonly IGelfConverter converter;
        private readonly LazyRetry<ITransport> transport;

        public GraylogSink(GraylogSinkOptions options)
        {
            transport = new LazyRetry<ITransport>(() => TransportFactory.FromOptions(options));
            converter = options.GelfConverter ?? GelfConverterFactory.FromOptions(options);
        }

        public void Emit(LogEvent @event)
        {
            try
            {
                var jObject = converter.GetGelfJson(@event);
                var json = jObject.ToString(Newtonsoft.Json.Formatting.None);
                transport.Value.SendAsync(json).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
            }
        }
    }
}
