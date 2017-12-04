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
        private readonly IDictionary<BuilderType, Lazy<IMessageBuilder>> messageBuilders;

        public GelfConverter(IDictionary<BuilderType, Lazy<IMessageBuilder>> messageBuilders)
        {
            this.messageBuilders = messageBuilders;
        }

        public JObject GetGelfJson(LogEvent logEvent)
        {
            var builder = logEvent.Exception != null
                ? messageBuilders[BuilderType.Exception].Value
                : messageBuilders[BuilderType.Message].Value;

            return builder.Build(logEvent);
        }
    }
}