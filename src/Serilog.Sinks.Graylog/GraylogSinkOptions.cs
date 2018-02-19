using System;
using System.Net.Http;
using System.Net.Sockets;
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
        public const int DefaultBatchPostingLimit = 50;
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);
        public const string DefaultFacility = "GELF";
        public const int DefaultShortMessageMaxLength = 500;
        public const LogEventLevel DefaultMinimumLogEventLevel = LevelAlias.Minimum;
        public const int DefaultStackTraceDepth = 10;
        public const MessageIdGeneratortype DefaultMessageGeneratorType = MessageIdGeneratortype.Timestamp;
        public const int DefaultPort = 9200;
        public const bool DefaultUseBatchMode = true;

        private static readonly IPropertyNamingStrategy DefaultPropertyNamingStrategy =
            new NoOpPropertyNamingStrategy();

        public GraylogSinkOptions()
        {
            MessageGeneratorType = MessageIdGeneratortype.Timestamp;
            ShortMessageMaxLength = DefaultShortMessageMaxLength;
            MinimumLogEventLevel = DefaultMinimumLogEventLevel;
            Facility = DefaultFacility;
            StackTraceDepth = DefaultStackTraceDepth;
            PropertyNamingStrategy = DefaultPropertyNamingStrategy;
            Period = DefaultPeriod;
            BatchSizeLimit = DefaultBatchPostingLimit;
            MessageGeneratorType = DefaultMessageGeneratorType;
            Port = DefaultPort;
            UseBatchMode = DefaultUseBatchMode;
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
        /// Gets or sets the Sink Batch Posting Limit
        /// </summary>
        /// <value>
        /// The Sink's Batch Posting Limit
        /// </value>
        public int BatchSizeLimit { get; set; }

        /// <summary>
        /// Gets or sets the Batch Send Interval
        /// </summary>
        /// <value>
        /// The Sink's Send Interval
        /// </value>
        public TimeSpan Period { get; set; }

        /// <summary>
        /// Gets or sets a user provided Http Client factory
        /// If you use a dependency injection container and would like to set your own lifetime management
        /// for the HTTP Trapnsport's HTTP Client, set it here.
        /// </summary>
        /// <value>
        /// The HttpClientFactory used by the HTTP Transport
        /// </value>
        public Func<HttpClient> HttpClientFactory { get; set; }

        /// <summary>
        /// Gets or sets a user provided Udp Client factory
        /// If you use a dependency injection container and would like to set your own lifetime management
        /// for the UDP Trapnsport's UDP Client, set it here.
        /// </summary>
        /// <value>
        /// The UdpClientFactory used by the UDP Transport
        /// </value>
        public Func<UdpClient> UdpClientFactory { get; set; }

        /// <summary>
        /// Uses a Periodic Batch Sink to send messages for slower Graylog endpoints
        /// </summary>
        /// <value>
        /// True if batch mode should be used
        /// </value>
        public bool UseBatchMode { get; set; }
    }
}