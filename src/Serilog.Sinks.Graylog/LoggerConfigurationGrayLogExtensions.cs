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
        /// <param name="useSsl">Use SSL in Tcp and Http</param>
        /// <param name="minimumLogEventLevel">The minimum log event level.</param>
        /// <param name="messageIdGeneratorType">Type of the message identifier generator.</param>
        /// <param name="shortMessageMaxLength">Short length of the message maximum.</param>
        /// <param name="stackTraceDepth">The stack trace depth.</param>
        /// <param name="facility">The facility.</param>
        /// <param name="maxMessageSizeInUdp">the maxMessageSizeInUdp</param>
        /// <param name="host">The host property to use in GELF message. If null, DNS hostname will be used instead.</param>
        /// <param name="includeMessageTemplate">if set to <c>true</c> if include message template to graylog.</param>
        /// <param name="messageTemplateFieldName">Name of the message template field.</param>
        /// <param name="usernameInHttp">The usernameInHttp. Basic authentication property.</param>
        /// <param name="passwordInHttp">The passwordInHttp. Basic authentication property.</param>
        /// <returns></returns>
        public static LoggerConfiguration Graylog(this LoggerSinkConfiguration loggerSinkConfiguration,
                                                  string hostnameOrAddress,
                                                  int port,
                                                  TransportType transportType,
                                                  bool useSsl,
                                                  LogEventLevel minimumLogEventLevel = LevelAlias.Minimum,
                                                  MessageIdGeneratorType messageIdGeneratorType = GraylogSinkOptionsBase.DefaultMessageGeneratorType,
                                                  int shortMessageMaxLength = GraylogSinkOptionsBase.DefaultShortMessageMaxLength,
                                                  int stackTraceDepth = GraylogSinkOptionsBase.DefaultStackTraceDepth,
                                                  string facility = GraylogSinkOptionsBase.DefaultFacility,
                                                  int maxMessageSizeInUdp = GraylogSinkOptionsBase.DefaultMaxMessageSizeInUdp,
                                                  string host = GraylogSinkOptionsBase.DefaultHost,
                                                  bool includeMessageTemplate = false,
                                                  string messageTemplateFieldName = GraylogSinkOptionsBase.DefaultMessageTemplateFieldName,
                                                  string usernameInHttp = null,
                                                  string passwordInHttp = null
                                                  )
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var options = new GraylogSinkOptions();
            options.HostnameOrAddress = hostnameOrAddress.Expand();
            options.Port = port;
            options.TransportType = transportType;
            options.UseSsl = useSsl;
            options.MinimumLogEventLevel = minimumLogEventLevel;
            options.MessageGeneratorType = messageIdGeneratorType;
            options.ShortMessageMaxLength = shortMessageMaxLength;
            options.StackTraceDepth = stackTraceDepth;
            options.Facility = facility.Expand();
            options.MaxMessageSizeInUdp = maxMessageSizeInUdp;
            options.Host = host;
            options.IncludeMessageTemplate = includeMessageTemplate;
            options.MessageTemplateFieldName = messageTemplateFieldName;
            options.UsernameInHttp = usernameInHttp;
            options.PasswordInHttp = passwordInHttp;
            return loggerSinkConfiguration.Graylog(options);
        }
    }
}