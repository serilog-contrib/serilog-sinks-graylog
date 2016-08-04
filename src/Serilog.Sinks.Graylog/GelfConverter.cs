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
        private readonly IDictionary<BuilderType, IMessageBuilder> _messageBuilders;

        public GelfConverter(IDictionary<BuilderType, IMessageBuilder> messageBuilders)
        {
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