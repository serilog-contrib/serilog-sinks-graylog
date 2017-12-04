using System;
using System.Collections.Generic;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.Graylog.Tests
{
    public class LogEventSource
    {
        public static LogEvent GetSimpleLogEvent(DateTimeOffset date)
        {
            var logEvent = new LogEvent(date, LogEventLevel.Information, null,
                new MessageTemplate("abcdef{TestProp}", new List<MessageTemplateToken>
                {
                    new TextToken("abcdef", 0),
                    new PropertyToken("TestProp", "zxc", alignment:new Alignment(AlignmentDirection.Left, 3))

                }), new List<LogEventProperty>
                {
                    new LogEventProperty("TestProp", new ScalarValue("zxc")),
                    new LogEventProperty("id", new ScalarValue("asd"))
                });
            return logEvent;
        }

        public  static LogEvent GetErrorEvent(DateTimeOffset date)
        {
            var logEvent = new LogEvent(date, LogEventLevel.Information, new InvalidCastException("Some errror"),
                new MessageTemplate("", new List<MessageTemplateToken>()),
                new List<LogEventProperty>(new List<LogEventProperty>()));
            return logEvent;
        }

        public static LogEvent GetComplexEvent(DateTimeOffset date)
        {
            var logEvent = new LogEvent(date, LogEventLevel.Information, null,
                new MessageTemplate("abcdef{TestProp}", new List<MessageTemplateToken>
                {
                    new TextToken("abcdef", 0),
                    new PropertyToken("TestProp", "zxc", alignment:new Alignment(AlignmentDirection.Left, 3))

                }), new List<LogEventProperty>
                {
                    new LogEventProperty("TestProp", new ScalarValue("zxc")),
                    new LogEventProperty("id", new ScalarValue("asd")),
                    new LogEventProperty("StructuredProperty",
                        new StructureValue(new List<LogEventProperty>
                        {
                            new LogEventProperty("id", new ScalarValue(1)),
                            new LogEventProperty("_TestProp", new ScalarValue(3))
                        }, "TypeTag"))
                });
            return logEvent;
        }

        public static LogEvent GetExceptionLogEvent(DateTimeOffset date, Exception testExc)
        {
            var logevent = new LogEvent(date, LogEventLevel.Error, testExc, new MessageTemplate("", new List<MessageTemplateToken>()),
                new List<LogEventProperty>(new List<LogEventProperty>()));
            return logevent;
        }
    }
}