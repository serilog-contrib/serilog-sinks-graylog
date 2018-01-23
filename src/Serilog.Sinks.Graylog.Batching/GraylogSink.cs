using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Graylog.Batching
{
    public class PeriodicBatchingGraylogSink: PeriodicBatchingSink
    {
        public PeriodicBatchingGraylogSink(int batchSizeLimit, TimeSpan period) : base(batchSizeLimit, period)
        {
        }

        public PeriodicBatchingGraylogSink(int batchSizeLimit, TimeSpan period, int queueLimit) : base(batchSizeLimit, period, queueLimit)
        {
        }

        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            base.EmitBatch(events);
        }

        protected override Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            return base.EmitBatchAsync(events);
        }
    }
}