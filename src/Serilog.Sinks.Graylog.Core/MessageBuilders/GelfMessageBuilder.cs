using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Graylog.Core.Extensions;
using Serilog.Sinks.Graylog.Core.Helpers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Serilog.Sinks.Graylog.Core.MessageBuilders
{
    using System.ComponentModel.Design;

    /// <summary>
    /// Message builder
    /// </summary>
    /// <seealso cref="IMessageBuilder" />
    public class GelfMessageBuilder : IMessageBuilder
    {
        
        private readonly string _hostName;
        private const string DefaultGelfVersion = "1.1";
        protected GraylogSinkOptionsBase Options { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GelfMessageBuilder"/> class.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="options">The options.</param>
        public GelfMessageBuilder(string hostName, GraylogSinkOptionsBase options)
        {
            _hostName = hostName;
            Options = options;
        }

        /// <summary>
        /// Builds the specified log event.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns></returns>
        public virtual JsonObject Build(LogEvent logEvent)
        {
            string message = logEvent.RenderMessage();
            string shortMessage = message.Truncate(Options.ShortMessageMaxLength);

            var jsonObject = new JsonObject
            {
                ["version"] = DefaultGelfVersion,
                ["host"] = Options.HostnameOverride ?? _hostName,
                ["short_message"] = shortMessage,
                ["timestamp"] = logEvent.Timestamp.ConvertToNix(),
                ["level"] = LogLevelMapper.GetMappedLevel(logEvent.Level),
                ["_stringLevel"] = logEvent.Level.ToString(),
                ["_facility"] = Options?.Facility
            };
            
            if (message.Length > Options.ShortMessageMaxLength)
            {
                jsonObject.Add("full_message", message);
            }

            foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
            {
                if (Options.ExcludeMessageTemplateProperties)
                {
                    var propertyTokens = logEvent.MessageTemplate.Tokens.OfType<PropertyToken>();
                    if (propertyTokens.Any(x => x.PropertyName == property.Key))
                    {
                        continue;
                    }
                }

                AddAdditionalField(jsonObject, property);
            }

            if (Options.IncludeMessageTemplate)
            {
                string messageTemplate = logEvent.MessageTemplate.Text;
                jsonObject.Add($"_{Options.MessageTemplateFieldName}", messageTemplate);
            }

            return jsonObject;
        }

        private void AddAdditionalField(JsonObject jObject,
                                        KeyValuePair<string, LogEventPropertyValue> property,
                                        string memberPath = "" )
        {
            string key = string.IsNullOrEmpty(memberPath)
                ? property.Key
                : $"{memberPath}.{property.Key}";

            switch (property.Value)
            {
                case ScalarValue scalarValue:
                    if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        key = "id_";
                    }

                    if (!key.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                    {
                        key = $"_{key}";
                    }

                    if (scalarValue.Value == null)
                    {
                        jObject.Add(key, null);
                        break;
                    }

                    var node = JsonSerializer.SerializeToNode(scalarValue.Value, Options.JsonSerializerOptions);
                    jObject.Add(key, node);
                    break;
                case SequenceValue sequenceValue:
                    var sequenceValueString = RenderPropertyValue(sequenceValue);
                    jObject.Add(key, sequenceValueString);
                    if (Options.ParseArrayValues)
                    {
                        int counter = 0;
                        foreach (var sequenceElement in sequenceValue.Elements)
                        {
                            AddAdditionalField(jObject, new KeyValuePair<string, LogEventPropertyValue>(counter.ToString(), sequenceElement), key);
                            counter++;
                        }
                    }
                    break;
                case StructureValue structureValue:
                    foreach (LogEventProperty logEventProperty in structureValue.Properties)
                    {
                        AddAdditionalField(jObject,
                                           new KeyValuePair<string, LogEventPropertyValue>(logEventProperty.Name, logEventProperty.Value),
                                           key);
                    }
                    break;
                case DictionaryValue dictionaryValue:
                    if (Options.ParseArrayValues)
                    {
                        foreach (KeyValuePair<ScalarValue, LogEventPropertyValue> dictionaryValueElement in dictionaryValue.Elements)
                        {
                            var renderedKey = RenderPropertyValue(dictionaryValueElement.Key);
                            AddAdditionalField(jObject, new KeyValuePair<string, LogEventPropertyValue>(renderedKey, dictionaryValueElement.Value), key);
                        }
                    }
                    else
                    {
                        var dict = dictionaryValue.Elements.ToDictionary(k => k.Key.Value, v => RenderPropertyValue(v.Value));
                        var stringDictionary = JsonSerializer.SerializeToNode(dict, Options.JsonSerializerOptions);
                        jObject.Add(key, stringDictionary);
                    }
                    break;
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