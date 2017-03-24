using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Sinks.Graylog.MessageBuilders;

namespace Serilog.Sinks.Graylog
{
    public interface IGelfConverter
    {
        JObject GetGelfJson(LogEvent logEvent);
    }

    public class GelfConverter : IGelfConverter
    {
        private IDictionary<BuilderType, Lazy<IMessageBuilder>> _messageBuilders;

        public GelfConverter(IDictionary<BuilderType, Lazy<IMessageBuilder>> messageBuilders)
        {
            _messageBuilders = messageBuilders;
        }

        public JObject GetGelfJson(LogEvent logEvent)
        {
            IMessageBuilder builder = logEvent.Exception != null
                ? _messageBuilders[BuilderType.Exception].Value
                : _messageBuilders[BuilderType.Message].Value;

            return builder.Build(logEvent);
        }
    }
}