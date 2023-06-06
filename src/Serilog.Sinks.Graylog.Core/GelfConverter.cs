using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core.MessageBuilders;

namespace Serilog.Sinks.Graylog.Core
{
    public interface IGelfConverter
    {
        JsonObject GetGelfJson(LogEvent logEvent);
    }

    public class GelfConverter : IGelfConverter
    {
        private readonly IDictionary<BuilderType, Lazy<IMessageBuilder>> _messageBuilders;

        public GelfConverter(IDictionary<BuilderType, Lazy<IMessageBuilder>> messageBuilders)
        {
            _messageBuilders = messageBuilders;
        }

        public JsonObject GetGelfJson(LogEvent logEvent)
        {
            IMessageBuilder builder = logEvent.Exception != null
                ? _messageBuilders[BuilderType.Exception].Value
                : _messageBuilders[BuilderType.Message].Value;

            return builder.Build(logEvent);
        }
    }
}