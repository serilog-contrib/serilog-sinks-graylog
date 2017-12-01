﻿using System;
using Serilog.Events;
using Serilog.Sinks.Graylog.Helpers;
using Serilog.Sinks.Graylog.MessageBuilders.PropertyNaming;
using Serilog.Sinks.Graylog.Transport;

namespace Serilog.Sinks.Graylog
{
    /// <summary>
    /// Sync options for graylog
    /// </summary>
    public class GraylogSinkOptions
    {
        internal const string DefaultFacility = "GELF";
        internal const int DefaultShortMessageMaxLength = 500;
        internal const LogEventLevel DefaultMinimumLogEventLevel = LevelAlias.Minimum;
        internal const int DefaultStackTraceDepth = 10;
        internal const MessageIdGeneratortype DefaultMessageGeneratorType = MessageIdGeneratortype.Timestamp;

        internal static readonly IPropertyNamingStrategy DefaultPropertyNamingStrategy =
            new NoOpPropertyNamingStrategy();

        public GraylogSinkOptions()
        {
            MessageGeneratorType = MessageIdGeneratortype.Timestamp;
            ShortMessageMaxLength = DefaultShortMessageMaxLength;
            MinimumLogEventLevel = DefaultMinimumLogEventLevel;
            //Spec says: facility must be set by the client to "GELF" if empty
            Facility = DefaultFacility;
            StackTraceDepth = DefaultStackTraceDepth;
            PropertyNamingStrategy = DefaultPropertyNamingStrategy;
            ThrowOnSendError = true;
        }

        /// <summary>
        /// Gets or sets the minimum log event level.
        /// </summary>
        /// <value>
        /// The minimum log event level.
        /// </value>
        public LogEventLevel MinimumLogEventLevel { get; set; }

        /// <summary>
        /// Gets or sets the hostname or address of graylog server.
        /// </summary>
        /// <value>
        /// The hostname or address.
        /// </value>
        public string HostnameOrAddress { get; set; }

        /// <summary>
        /// Gets or sets the hostname or address of graylog server.
        /// </summary>
        /// <value>
        /// The hostname or address.
        /// </value>
        [Obsolete]
        public string HostnameOrAdress
        {
            get => HostnameOrAddress;
            set => HostnameOrAddress = value;
        }

        /// <summary>
        /// Gets or sets the facility name.
        /// </summary>
        /// <value>
        /// The facility.
        /// </value>
        public string Facility { get; set; }

        /// <summary>
        /// Gets or sets the server port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the transport.
        /// </summary>
        /// <value>
        /// The transport.
        /// </value>
        /// <remarks>
        /// You can implement another one or use default udp transport
        /// </remarks>
        public TransportType TransportType { get; set; }

        /// <summary>
        /// Gets or sets the gelf converter.
        /// </summary>
        /// <value>
        /// The gelf converter.
        /// </value>
        /// <remarks>
        /// You can implement another one for customize fields or use default
        /// </remarks>
        public IGelfConverter GelfConverter { get; set; }

        /// <summary>
        /// Gets or sets the maximal length of the ShortMessage
        /// </summary>
        /// <value>
        /// ShortMessage Length
        /// </value>
        public int ShortMessageMaxLength { get; set; }

        /// <summary>
        /// Gets or sets the type of the message generator.
        /// </summary>
        /// <value>
        /// The type of the message generator.
        /// </value>
        /// <remarks>
        /// its timestamp or first 8 bytes of md5 hash
        /// </remarks>
        public MessageIdGeneratortype MessageGeneratorType { get; set; }

        /// <summary>
        /// Gets or sets the stack trace depth.
        /// </summary>
        /// <value>
        /// The stack trace depth.
        /// </value>
        public int StackTraceDepth { get; set; }

        /// <summary>
        /// Gets or sets the property naming strategy.
        /// </summary>
        /// <value>
        /// The property naming strategy.
        /// </value>
        public IPropertyNamingStrategy PropertyNamingStrategy { get; set; }

        /// <summary>
        /// Indicates if the Sink should propogate send errors.
        /// </summary>
        /// <value>
        /// True if errors should be rethrown and propogated up.
        /// 
        public bool ThrowOnSendError { get; set; }
    }
}