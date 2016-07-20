using System;
using Serilog.Events;

namespace Serilog.Sinks.Graylog
{
    public class GraylogSinkOptions
    {
        public int BatchSizeLimit { get; set; }
        public TimeSpan Period { get; set; }
        public LogEventLevel? MinimumLogEventLevel { get; set; }
        public string HostnameOrAdress { get; set; }
        public string Facility { get; set; }
        public int Port { get; set; }
    }
}