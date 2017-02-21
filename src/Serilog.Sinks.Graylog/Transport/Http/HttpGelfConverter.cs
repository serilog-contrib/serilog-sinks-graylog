using System.Net;
using Newtonsoft.Json.Linq;
using Serilog.Events;

namespace Serilog.Sinks.Graylog.Transport.Http
{
    public class HttpGelfConverter : IGelfConverter
    {
        private const int ShortMessageLength = 250;
        private static readonly string HostName = Dns.GetHostName();

        private readonly string _facility;
        private readonly int _shortMessageMaxLength;

        public HttpGelfConverter(string facility, int shortMessageMaxLength = ShortMessageLength)
        {
            _facility = facility;
            _shortMessageMaxLength = shortMessageMaxLength;
        }

        public JObject GetGelfJson(LogEvent logEventInfo)
        {
            var formattedMessage = logEventInfo.MessageTemplate.Render(logEventInfo.Properties);
            var shortMessage = formattedMessage.Length > _shortMessageMaxLength ? formattedMessage.Substring(0, _shortMessageMaxLength - 1) : formattedMessage;

            var json = new JObject();
            Add(json, "short_message", shortMessage);
            Add(json, "full_message", formattedMessage);
            Add(json, "host", HostName);
            Add(json, "level", ((int)logEventInfo.Level).ToString());
            Add(json, "facility", _facility);
            Add(json, "_levelName", logEventInfo.Level.ToString());

            if (logEventInfo.Exception != null)
            {
                var exceptioToLog = logEventInfo.Exception;
                while (exceptioToLog.InnerException != null)
                {
                    exceptioToLog = exceptioToLog.InnerException;
                }

                Add(json, "_exception_type", exceptioToLog.GetType().Name);
                Add(json, "_exception_message", exceptioToLog.Message);
                Add(json, "_exception_stack_trace", exceptioToLog.StackTrace);
            }

            if (logEventInfo.Properties != null)
            {
                foreach (var pair in logEventInfo.Properties)
                {
                    Add(json, "_" + pair.Key, pair.Value.ToString());
                }
            }

            return json;
        }

        private void Add(JObject json, string property, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            json.Add(property, value);
        }
    }
}
