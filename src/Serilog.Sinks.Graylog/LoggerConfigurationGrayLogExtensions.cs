using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Graylog.Helpers;
using Serilog.Sinks.Graylog.Transport;

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

        public static LoggerConfiguration Graylog(this LoggerSinkConfiguration loggerSinkConfiguration,
            string hostnameOrAddress,
            int port,
            TransportType transportType,
            LogEventLevel minimumLogEventLevel = LevelAlias.Minimum,
            MessageIdGeneratortype messageIdGeneratorType = GraylogSinkOptions.DefaultMessageGeneratorType,
            int shortMessageMaxLength = GraylogSinkOptions.DefaultShortMessageMaxLength,
            int stackTraceDepth = GraylogSinkOptions.DefaultStackTraceDepth,
            string facility = GraylogSinkOptions.DefaultFacility
            )
        {
            var options = new GraylogSinkOptions
            {
                HostnameOrAdress = hostnameOrAddress,
                Port = port,
                TransportType = transportType,
                MinimumLogEventLevel = minimumLogEventLevel,
                MessageGeneratorType = messageIdGeneratorType,
                ShortMessageMaxLength = shortMessageMaxLength,
                StackTraceDepth = stackTraceDepth,
                Facility = facility
            };

            return loggerSinkConfiguration.Graylog(options);
        }
    }
}