using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Serilog.Events;

namespace Serilog.Sinks.Graylog.Core.MessageBuilders
{
    /// <summary>
    /// Exception builder
    /// </summary>
    /// <seealso cref="GelfMessageBuilder" />
    public class ExceptionMessageBuilder : GelfMessageBuilder
    {
        private const string ExceptionDelimiter = " - ";
        private const string StackTraceDelimiter = "--- Inner exception stack trace ---";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMessageBuilder"/> class.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="options">The options.</param>
        public ExceptionMessageBuilder(string hostName, GraylogSinkOptions options) : base(hostName, options)
        {
        }

        public override JObject Build(LogEvent logEvent)
        {
            Tuple<string, string> excMessageTuple = GetExceptionMessages(logEvent.Exception);
            string exceptionDetail = excMessageTuple.Item1;
            string stackTrace = excMessageTuple.Item2;

            logEvent.AddOrUpdateProperty(new LogEventProperty("ExceptionSource", new ScalarValue(logEvent.Exception.Source)));
            logEvent.AddOrUpdateProperty(new LogEventProperty("ExceptionMessage", new ScalarValue(exceptionDetail)));
            logEvent.AddOrUpdateProperty(new LogEventProperty("StackTrace", new ScalarValue(stackTrace)));

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
                exceptionSb.Append(nestedException.Message).Append(ExceptionDelimiter);
                if (nestedException.StackTrace != null)
                {
                    stackSb.AppendLine(nestedException.StackTrace).AppendLine(StackTraceDelimiter);
                }
                nestedException = nestedException.InnerException;
                counter++;
            }
            while (nestedException != null && counter < Options.StackTraceDepth);

            string exceptionDetail = exceptionSb.ToString().Substring(0, exceptionSb.Length - ExceptionDelimiter.Length).Trim();

            if (stackSb.Length > 0)
            {
                stackDetail = stackSb.ToString().Substring(0, stackSb.Length - StackTraceDelimiter.Length - 2).Trim();
            }

            return new Tuple<string, string>(exceptionDetail, stackDetail);
        }

    }
}