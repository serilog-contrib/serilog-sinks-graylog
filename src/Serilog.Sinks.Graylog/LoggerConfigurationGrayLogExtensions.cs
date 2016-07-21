using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Graylog
{
    public static class LoggerConfigurationGrayLogExtensions
    {
        public static LoggerConfiguration Graylog(this LoggerSinkConfiguration loggerSinkConfiguration,
            GraylogSinkOptions options)
        {
            var sink = (ILogEventSink) new GraylogSink(options);
            return loggerSinkConfiguration.Sink(sink, options.MinimumLogEventLevel);
        }
    }
}