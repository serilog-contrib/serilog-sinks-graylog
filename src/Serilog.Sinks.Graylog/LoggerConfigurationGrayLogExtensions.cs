using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Graylog.Helpers;
using Serilog.Sinks.Graylog.Transport;

namespace Serilog.Sinks.Graylog
{
    public static class LoggerConfigurationGrayLogExtensions
    {
        /// <summary>
        /// Graylogs the specified options.
        /// </summary>
        /// <param name="loggerSinkConfiguration">The logger sink configuration.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static LoggerConfiguration Graylog(this LoggerSinkConfiguration loggerSinkConfiguration,
            GraylogSinkOptions options)
        {
            var sink = (ILogEventSink)new GraylogSink(options);
            return loggerSinkConfiguration.Sink(sink, options.MinimumLogEventLevel);
        }

        /// <summary>
        /// Graylogs the specified hostname or address.
        /// </summary>
        /// <param name="loggerSinkConfiguration">The logger sink configuration.</param>
        /// <param name="hostnameOrAddress">The hostname or address.</param>
        /// <param name="port">The port.</param>
        /// <param name="transportType">Type of the transport.</param>
        /// <param name="minimumLogEventLevel">The minimum log event level.</param>
        /// <param name="messageIdGeneratorType">Type of the message identifier generator.</param>
        /// <param name="shortMessageMaxLength">Short length of the message maximum.</param>
        /// <param name="stackTraceDepth">The stack trace depth.</param>
        /// <param name="facility">The facility.</param>
        /// <returns></returns>
        public static LoggerConfiguration Graylog(this LoggerSinkConfiguration loggerSinkConfiguration,
                                                  string hostnameOrAddress,
                                                  int port,
                                                  TransportType transportType,
                                                  LogEventLevel minimumLogEventLevel = LevelAlias.Minimum,
                                                  MessageIdGeneratortype messageIdGeneratorType = GraylogSinkOptions.DefaultMessageGeneratorType,
                                                  int shortMessageMaxLength = GraylogSinkOptions.DefaultShortMessageMaxLength,
                                                  int stackTraceDepth = GraylogSinkOptions.DefaultStackTraceDepth,
                                                  string facility = GraylogSinkOptions.DefaultFacility)
        {
            var options = new GraylogSinkOptions
            {
                HostnameOrAddress = hostnameOrAddress,
                Port = port,
                TransportType = transportType,
                MinimumLogEventLevel = minimumLogEventLevel,
                MessageGeneratorType = messageIdGeneratorType,
                ShortMessageMaxLength = shortMessageMaxLength,
                StackTraceDepth = stackTraceDepth,
                Facility = facility,
                PropertyNamingStrategy = GraylogSinkOptions.DefaultPropertyNamingStrategy
            };

            return loggerSinkConfiguration.Graylog(options);
        }
    }
}