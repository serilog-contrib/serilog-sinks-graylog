using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Sinks.Graylog.Extensions;
using Serilog.Sinks.Graylog.MessageBuilder;

namespace Serilog.Sinks.Graylog.MessageBuilders
{
    public class GelfMessageBuilder : IMessageBuilder
    {
        private readonly string _hostName;
        private const string GelfVersion = "1.1";
        protected GraylogSinkOptions Options { get; }


        public GelfMessageBuilder(string hostName, GraylogSinkOptions options)
        {
            _hostName = hostName;
            Options = options;
        }

        public virtual JObject Build(LogEvent logEvent)
        {
            string message = logEvent.RenderMessage();
            string shortMessage = message.ShortMessage(Options.ShortMessageMaxLength);

            var gelfMessage = new GelfMessage
            {
                Version = GelfVersion,
                Host = _hostName,
                ShortMessage = shortMessage,
                FullMessage = message,
                Timestamp = logEvent.Timestamp.DateTime,
                Level = (int)logEvent.Level,
                StringLevel = logEvent.Level.ToString(),
                Facility = Options.Facility
            };

            JObject jsonObject = JObject.FromObject(gelfMessage);
            foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
            {
                AddAdditionalField(jsonObject, property);
            }
            return jsonObject;
        }

        private void AddAdditionalField(IDictionary<string, JToken> jObject, KeyValuePair<string, LogEventPropertyValue> property, int recursionLevel = 0, string typeTag = null)
        {
            if (property.Value is ScalarValue)
            {
                string key = property.Key;

                if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                    key = "id_";
                if (!key.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                    key = "_" + key;


                key = recursionLevel > 0 ? $"{typeTag}{key}" : key;


                LogEventPropertyValue logEventProperty = property.Value;
                string stringValue = logEventProperty.ToString();

                var value = JToken.FromObject(stringValue);

                jObject.Add(key, value);
            }

            var structureValue = property.Value as StructureValue;
            if (structureValue == null)
            {
                return;
            }
            StructureValue structuredValue = structureValue;
            foreach (LogEventProperty logEventProperty in structuredValue.Properties)
            {
                AddAdditionalField(jObject, new KeyValuePair<string, LogEventPropertyValue>(logEventProperty.Name, logEventProperty.Value), ++recursionLevel, structureValue.TypeTag);
            }
        }

    }
}