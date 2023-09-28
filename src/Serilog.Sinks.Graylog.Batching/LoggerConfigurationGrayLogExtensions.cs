using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Helpers;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.PeriodicBatching;
using System;

namespace Serilog.Sinks.Graylog.Batching
{
    public static class LoggerConfigurationGrayLogExtensions
    {
        /// <param name="loggerSinkConfiguration">The logger sink configuration.</param>
        /// <param name="options">The options.</param>
        public static LoggerConfiguration Graylog(this LoggerSinkConfiguration loggerSinkConfiguration,
                                                  BatchingGraylogSinkOptions options)
        {
            var sink = new PeriodicBatchingGraylogSink(options);

            var batchingSink = new PeriodicBatchingSink(sink, options.PeriodicOptions);

            return loggerSinkConfiguration.Sink(batchingSink, options.MinimumLogEventLevel);
        }

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
            var options = new BatchingGraylogSinkOptions
            {
                HostnameOrAddress = hostnameOrAddress,
                Port = port,
                TransportType = transportType,
                MinimumLogEventLevel = minimumLogEventLevel,
                MessageGeneratorType = messageIdGeneratorType,
                ShortMessageMaxLength = shortMessageMaxLength,
                StackTraceDepth = stackTraceDepth,
                Facility = facility,
                PeriodicOptions = new PeriodicBatchingSinkOptions()
                {
                    BatchSizeLimit = batchSizeLimit,
                    Period = period,
                    QueueLimit = queueLimit,
                },
                MaxMessageSizeInUdp = maxMessageSizeInUdp,
                IncludeMessageTemplate = includeMessageTemplate,
                MessageTemplateFieldName = messageTemplateFieldName
            };
            return loggerSinkConfiguration.Graylog(options);
        }
    }
}
