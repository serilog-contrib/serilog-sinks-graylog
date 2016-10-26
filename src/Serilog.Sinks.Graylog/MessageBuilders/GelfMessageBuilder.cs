using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Sinks.Graylog.Extensions;
using Serilog.Sinks.Graylog.Helpers;

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
                Level = LogLevelMapper.GetMappedLevel(logEvent.Level),
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

        private void AddAdditionalField(IDictionary<string, JToken> jObject, KeyValuePair<string, LogEventPropertyValue> property, string memberPath = "")
        {
            string key = string.IsNullOrEmpty(memberPath)
                ? property.Key
                : $"{memberPath}.{property.Key}";

            if (property.Value is ScalarValue)
            {

                if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                    key = "id_";
                if (!key.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                    key = "_" + key;

                LogEventPropertyValue logEventProperty = property.Value;

                string stringValue = RenderPropertyValue(logEventProperty);
                JToken value = JToken.FromObject(stringValue);

                jObject.Add(key, value);
            }

            var sequenceValue = property.Value as SequenceValue;
            if (sequenceValue != null)
            {
                var sequenceValuestring = RenderPropertyValue(sequenceValue);
                jObject.Add(key, sequenceValuestring);
            }


            var structureValue = property.Value as StructureValue;
            if (structureValue != null)
            {
                foreach (LogEventProperty logEventProperty in structureValue.Properties)
                {
                    AddAdditionalField(jObject,
                        new KeyValuePair<string, LogEventPropertyValue>(logEventProperty.Name, logEventProperty.Value), key);
                }
            }
        }

        private string RenderPropertyValue(LogEventPropertyValue propertyValue)
        {
            using (TextWriter tw = new StringWriter())
            {
                propertyValue.Render(tw);
                string result = tw.ToString();
                result = result.Trim('"');
                return result;
            }
        }

    }
}