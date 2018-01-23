using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Graylog.Batching
{
    public class PeriodicBatchingGraylogSink : PeriodicBatchingSink
    {
        private readonly ITransport _transport;
        private readonly IGelfConverter _converter;

        public PeriodicBatchingGraylogSink(BatchingGraylogSinkOptions options) : this(options, options.BatchSizeLimit, options.Period, options.QueueLimit)
        {

        }

        public PeriodicBatchingGraylogSink(GraylogSinkOptions options, int batchSizeLimit, TimeSpan period) : base(batchSizeLimit, period)
        {
            ISinkComponentsBuilder sinkComponentsBuilder = new SinkComponentsBuilder(options);
            _transport = sinkComponentsBuilder.MakeTransport();
            _converter = sinkComponentsBuilder.MakeGelfConverter();

        }

        public PeriodicBatchingGraylogSink(GraylogSinkOptions options, int batchSizeLimit, TimeSpan period, int queueLimit) : base(batchSizeLimit, period, queueLimit)
        {
            ISinkComponentsBuilder sinkComponentsBuilder = new SinkComponentsBuilder(options);
            _transport = sinkComponentsBuilder.MakeTransport();
            _converter = sinkComponentsBuilder.MakeGelfConverter();

        }

        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            IEnumerable<Task> sendTasks = events.Select(logEvent =>
            {
                JObject json = _converter.GetGelfJson(logEvent);
                Task resultTask = _transport.Send(json.ToString(Newtonsoft.Json.Formatting.None));
                return resultTask;
            });

            Task.WaitAll(sendTasks.ToArray());

            base.EmitBatch(events);
        }

        protected override Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            IEnumerable<Task> sendTasks = events.Select(logEvent =>
            {
                JObject json = _converter.GetGelfJson(logEvent);
                Task resultTask = _transport.Send(json.ToString(Newtonsoft.Json.Formatting.None));
                return resultTask;
            });

            return Task.WhenAll(sendTasks);
        }
    }
}