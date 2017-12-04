using System;
using System.Collections.Generic;
using System.Net;
using Serilog.Sinks.Graylog.MessageBuilders;

namespace Serilog.Sinks.Graylog
{
    public static class GelfConverterFactory
    {
        public static IGelfConverter FromOptions(GraylogSinkOptions options)
        {
            var hostName = Dns.GetHostName();
            var builders = new Dictionary<BuilderType, Lazy<IMessageBuilder>>
            {
                [BuilderType.Exception] =
                new Lazy<IMessageBuilder>(() => new ExceptionMessageBuilder(hostName, options)),
                [BuilderType.Message] =
                new Lazy<IMessageBuilder>(() => new GelfMessageBuilder(hostName, options))
            };

            return new GelfConverter(builders);
        }
    }
}
