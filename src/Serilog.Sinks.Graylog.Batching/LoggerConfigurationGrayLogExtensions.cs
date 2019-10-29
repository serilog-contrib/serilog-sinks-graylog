using System;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Helpers;
using Serilog.Sinks.Graylog.Core.Transport;

namespace Serilog.Sinks.Graylog.Batching
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
                                                  BatchingGraylogSinkOptions options)
        {
            var sink = (ILogEventSink)new PeriodicBatchingGraylogSink(options);
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
        /// <param name="batchSizeLimit">The batch size limit</param>
        /// <param name="period">The period limit default is one second</param>
        /// <param name="queueLimit">queue limit</param>
        /// <param name="maxMessageSizeInUdp">the maxMessageSizeInUdp</param>
        /// <param name="includeMessageTemplate">if set to <c>true</c> if include message template to graylog.</param>
        /// <param name="messageTemplateFieldName">Name of the message template field.</param>
        /// <returns></returns>
        public static LoggerConfiguration Graylog(this LoggerSinkConfiguration loggerSinkConfiguration,
                                                  string hostnameOrAddress,
                                                  int port,
                                                  TransportType transportType,
                                                  LogEventLevel minimumLogEventLevel = LevelAlias.Minimum,
                                                  MessageIdGeneratorType messageIdGeneratorType = GraylogSinkOptionsBase.DefaultMessageGeneratorType,
                                                  int shortMessageMaxLength = GraylogSinkOptionsBase.DefaultShortMessageMaxLength,
                                                  int stackTraceDepth = GraylogSinkOptionsBase.DefaultStackTraceDepth,
                                                  string facility = GraylogSinkOptionsBase.DefaultFacility,
                                                  int maxMessageSizeInUdp = GraylogSinkOptionsBase.DefaultMaxMessageSizeInUdp,
                                                  int batchSizeLimit = 10,
                                                  TimeSpan period = default,
                                                  int queueLimit = 1000,
                                                  bool includeMessageTemplate = false,
                                                  string messageTemplateFieldName = GraylogSinkOptionsBase.DefaultMessageTemplateFieldName
            )
        {
            if (period == default)
            {
                period = TimeSpan.FromSeconds(1);
            }

            // ReSharper disable once UseObjectOrCollectionInitializer
            var options = new BatchingGraylogSinkOptions();
            options.HostnameOrAddress = hostnameOrAddress;
            options.Port = port;
            options.TransportType = transportType;
            options.MinimumLogEventLevel = minimumLogEventLevel;
            options.MessageGeneratorType = messageIdGeneratorType;
            options.ShortMessageMaxLength = shortMessageMaxLength;
            options.StackTraceDepth = stackTraceDepth;
            options.Facility = facility;
            options.BatchSizeLimit = batchSizeLimit;
            options.Period = period;
            options.QueueLimit = queueLimit;
            options.MaxMessageSizeInUdp = maxMessageSizeInUdp;
            options.IncludeMessageTemplate = includeMessageTemplate;
            options.MessageTemplateFieldName = messageTemplateFieldName;

            return loggerSinkConfiguration.Graylog(options);
        }
    }
}