using System;
using Serilog.Sinks.Graylog.Core;

namespace Serilog.Sinks.Graylog.Batching
{
    public class BatchingGraylogSinkOptions : GraylogSinkOptionsBase
    {
        public BatchingGraylogSinkOptions()
        {
            BatchSizeLimit = 10;
            Period  = TimeSpan.FromSeconds(1);
            QueueLimit = 10;
        }

        public int BatchSizeLimit { get; set; }

        public TimeSpan Period { get; set; }

        public int QueueLimit { get; set; }
    }
}