using Newtonsoft.Json.Linq;
using Serilog.Events;

namespace Serilog.Sinks.Graylog.Core.MessageBuilders
{
    /// <summary>
    /// Build json message for graylog
    /// </summary>
    public interface IMessageBuilder
    {
        /// <summary>
        /// Builds the specified log event.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns></returns>
        JObject Build(LogEvent logEvent);
    }

    /// <summary>
    /// Builder type
    /// </summary>
    public enum BuilderType
    {
        /// <summary>
        /// Exception Builder
        /// </summary>
        Exception,
        /// <summary>
        /// Message Builder
        /// </summary>
        Message
    }
}