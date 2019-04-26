using System;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Extensions;
using Serilog.Sinks.Graylog.Core.Helpers;
using Serilog.Sinks.Graylog.Core.Transport;


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
        public static LoggerConfiguration Graylog(this LoggerSinkConfiguration loggerSinkConfiguration, GraylogSinkOptions options)
        {
            var sink = (ILogEventSink) new GraylogSink(options);
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
        /// <param name="maxMessageSizeInUdp">the maxMessageSizeInUdp</param>
        /// <returns></returns>
        public static LoggerConfiguration Graylog(this LoggerSinkConfiguration loggerSinkConfiguration,
                                                  string hostnameOrAddress,
                                                  int port,
                                                  TransportType transportType,
                                                  LogEventLevel minimumLogEventLevel = LevelAlias.Minimum,
                                                  MessageIdGeneratortype messageIdGeneratorType = GraylogSinkOptionsBase.DefaultMessageGeneratorType,
                                                  int shortMessageMaxLength = GraylogSinkOptionsBase.DefaultShortMessageMaxLength,
                                                  int stackTraceDepth = GraylogSinkOptionsBase.DefaultStackTraceDepth,
                                                  string facility = GraylogSinkOptionsBase.DefaultFacility,
                                                  int maxMessageSizeInUdp = GraylogSinkOptionsBase.DefaultMaxMessageSizeInUdp)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var options = new GraylogSinkOptions();
            options.HostnameOrAddress = hostnameOrAddress.Expand();
            options.Port = port;
            options.TransportType = transportType;
            options.MinimumLogEventLevel = minimumLogEventLevel;
            options.MessageGeneratorType = messageIdGeneratorType;
            options.ShortMessageMaxLength = shortMessageMaxLength;
            options.StackTraceDepth = stackTraceDepth;
            options.Facility = facility.Expand();
            options.MaxMessageSizeInUdp = maxMessageSizeInUdp;

            return loggerSinkConfiguration.Graylog(options);
        }
    }
}