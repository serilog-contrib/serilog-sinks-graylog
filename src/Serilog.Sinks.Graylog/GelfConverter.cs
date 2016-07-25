using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Sinks.Graylog.Extensions;
using Serilog.Sinks.Graylog.MessageBuilder;

namespace Serilog.Sinks.Graylog
{
    public interface IGelfConverter
    {
        JObject GetGelfJson(LogEvent logEvent);
    }

    public class GelfConverter : IGelfConverter
    {
        private readonly string _hostName;
        private readonly GraylogSinkOptions _options;
        private readonly IDictionary<BuilderType, IMessageBuilder> _messageBuilders;

        public GelfConverter(string hostName, GraylogSinkOptions options, IDictionary<BuilderType, IMessageBuilder> messageBuilders)
        {
            _hostName = hostName;
            _options = options;
            _messageBuilders = messageBuilders;
        }

        public JObject GetGelfJson(LogEvent logEvent)
        {
            IMessageBuilder builder = logEvent.Exception != null
                ? _messageBuilders[BuilderType.Exception]
                : _messageBuilders[BuilderType.Message];

            return builder.Build(logEvent);
        }
    }
}