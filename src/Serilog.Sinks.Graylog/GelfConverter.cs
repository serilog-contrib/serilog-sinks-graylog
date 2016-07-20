using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Serilog.Events;

namespace Serilog.Sinks.Graylog
{
    public class GelfConverter
    {
        private const int ShortMessageMaxLength = 500;
        private const string GelfVersion = "1.1";

        public JObject GetGelfJson(LogEvent logEvent, string facility)
        {
            //string message = logEvent.MessageTemplate.ToString();
            string message = logEvent.RenderMessage();
            if (logEvent.Exception != null)
            {
                string exceptionDetail;
                string stackDetail;

                GetExceptionMessages(logEvent.Exception, out exceptionDetail, out stackDetail);

                logEvent.AddOrUpdateProperty(new LogEventProperty("ExceptionSource", new ScalarValue("logEventInfo.Exception.Source")));
                logEvent.AddOrUpdateProperty(new LogEventProperty("ExceptionMessage", new ScalarValue(exceptionDetail)));
                logEvent.AddOrUpdateProperty(new LogEventProperty("StackTrace", new ScalarValue(stackDetail)));
            }
            string shortMessage = message;
            if (shortMessage.Length > ShortMessageMaxLength)
            {
                shortMessage = shortMessage.Substring(0, ShortMessageMaxLength);
            }

            var gelfMessage = new GelfMessage
            {
                Version = GelfVersion,
                Host = Dns.GetHostName(),
                ShortMessage = shortMessage,
                FullMessage = message,
                Timestamp = logEvent.Timestamp.DateTime,
                Level = (int)logEvent.Level,
                //Spec says: facility must be set by the client to "GELF" if empty
                Facility = (string.IsNullOrEmpty(facility) ? "GELF" : facility),
            };

            JObject jsonObject = JObject.FromObject(gelfMessage);
            foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
            {
                AddAdditionalField(jsonObject, property);
            }

            return jsonObject;
        }

        private void AddAdditionalField(IDictionary<string, JToken> jObject, KeyValuePair<string, LogEventPropertyValue> property)
        {
            if (property.Value is ScalarValue)
            {
                string key = property.Key;
                if (key == null) return;

                if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                    key = "id_";
                if (!key.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                    key = "_" + key;

                JToken value = null;
                if (property.Value != null)
                {
                    LogEventPropertyValue logEventProperty = property.Value;
                    string stringValue = logEventProperty.ToString();

                    value = JToken.FromObject(stringValue);
                }
                jObject.Add(key, value);
            }

            if (property.Value is StructureValue)
            {
                var structuredValue = (StructureValue) property.Value;
                foreach (LogEventProperty logEventProperty in structuredValue.Properties)
                {
                    AddAdditionalField(jObject, new KeyValuePair<string, LogEventPropertyValue>(logEventProperty.Name, logEventProperty.Value));
                }
            }
        }

        /// <summary>
        /// Get the message details from all nested exceptions, up to 10 in depth.
        /// </summary>
        /// <param name="ex">Exception to get details for</param>
        /// <param name="exceptionDetail">Exception message</param>
        /// <param name="stackDetail">Stacktrace with inner exceptions</param>
        private static void GetExceptionMessages(Exception ex, out string exceptionDetail, out string stackDetail)
        {
            var exceptionSb = new StringBuilder();
            var stackSb = new StringBuilder();
            var nestedException = ex;
            stackDetail = null;

            int counter = 0;
            do
            {
                exceptionSb.Append(nestedException.Message + " - ");
                if (nestedException.StackTrace != null)
                    stackSb.Append(nestedException.StackTrace + "--- Inner exception stack trace ---");
                nestedException = nestedException.InnerException;
                counter++;
            }
            while (nestedException != null && counter < 11);

            exceptionDetail = exceptionSb.ToString().Substring(0, exceptionSb.Length - 3);
            if (stackSb.Length > 0)
                stackDetail = stackSb.ToString().Substring(0, stackSb.Length - 35);
        }
    }
}