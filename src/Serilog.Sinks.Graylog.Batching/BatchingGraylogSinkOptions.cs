using System;
using Serilog.Sinks.Graylog.Core;

namespace Serilog.Sinks.Graylog.Batching
{
    public class BatchingGraylogSinkOptions : GraylogSinkOptions
    {
        public int BatchSizeLimit { get; set; }

        public TimeSpan Period { get; set; }

        public int QueueLimit { get; set; }
    }
}