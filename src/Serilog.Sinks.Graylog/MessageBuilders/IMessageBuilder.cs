using Newtonsoft.Json.Linq;
using Serilog.Events;

namespace Serilog.Sinks.Graylog.MessageBuilders
{
    public interface IMessageBuilder
    {
        JObject Build(LogEvent logEvent);
    }

    public enum BuilderType
    {
        Exception,
        Message
    }
}