using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Serilog.Events;

namespace Serilog.Sinks.Graylog.MessageBuilder
{
    public class ExceptionGelfMessageBuilder : MessageBuilders.GelfMessageBuilder
    {
        public override JObject Build(LogEvent logEvent)
        {
            Tuple<string, string> excMessageTuple = GetExceptionMessages(logEvent.Exception);
            string exceptionDetail = excMessageTuple.Item1;
            string stackDetail = excMessageTuple.Item2;

            logEvent.AddOrUpdateProperty(new LogEventProperty("ExceptionSource", new ScalarValue(logEvent.Exception.Source)));
            logEvent.AddOrUpdateProperty(new LogEventProperty("ExceptionMessage", new ScalarValue(exceptionDetail)));
            logEvent.AddOrUpdateProperty(new LogEventProperty("StackTrace", new ScalarValue(stackDetail)));

            return base.Build(logEvent);
        }

        /// <summary>
        /// Get the message details from all nested exceptions, up to 10 in depth.
        /// </summary>
        /// <param name="ex">Exception to get details for</param>
        private Tuple<string, string> GetExceptionMessages(Exception ex)
        {
            var exceptionSb = new StringBuilder();
            var stackSb = new StringBuilder();
            Exception nestedException = ex;
            string stackDetail = null;

            var counter = 0;
            do
            {
                exceptionSb.Append(nestedException.Message + " - ");
                if (nestedException.StackTrace != null)
                    stackSb.Append(nestedException.StackTrace + "--- Inner exception stack trace ---");
                nestedException = nestedException.InnerException;
                counter++;
            }
            while (nestedException != null && counter < Options.StackTraceDepth);

            string exceptionDetail = exceptionSb.ToString().Substring(0, exceptionSb.Length - 3);
            if (stackSb.Length > 0)
                stackDetail = stackSb.ToString().Substring(0, stackSb.Length - 35);

            return new Tuple<string, string>(exceptionDetail, stackDetail);
        }

        public ExceptionGelfMessageBuilder(string hostName, GraylogSinkOptions options) : base(hostName, options)
        {
        }
    }
}