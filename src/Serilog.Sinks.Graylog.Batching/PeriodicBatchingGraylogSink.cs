using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Graylog.Batching
{
    public class PeriodicBatchingGraylogSink : PeriodicBatchingSink
    {
        private readonly Lazy<ITransport> _transport;
        private readonly Lazy<IGelfConverter> _converter;

        public PeriodicBatchingGraylogSink(BatchingGraylogSinkOptions options) : this(options, options.BatchSizeLimit, options.Period, options.QueueLimit)
        {

        }

        public PeriodicBatchingGraylogSink(GraylogSinkOptions options, int batchSizeLimit, TimeSpan period) : base(batchSizeLimit, period)
        {
            ISinkComponentsBuilder sinkComponentsBuilder = new SinkComponentsBuilder(options);
            _transport = new Lazy<ITransport>(() => sinkComponentsBuilder.MakeTransport());
            _converter = new Lazy<IGelfConverter>(() => sinkComponentsBuilder.MakeGelfConverter());

        }

        public PeriodicBatchingGraylogSink(GraylogSinkOptions options, int batchSizeLimit, TimeSpan period, int queueLimit) : base(batchSizeLimit, period, queueLimit)
        {
            ISinkComponentsBuilder sinkComponentsBuilder = new SinkComponentsBuilder(options);
            _transport = new Lazy<ITransport>(() => sinkComponentsBuilder.MakeTransport());
            _converter = new Lazy<IGelfConverter>(() => sinkComponentsBuilder.MakeGelfConverter());
        }

        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            try
            {
                Task[] sendTasks = events.Select(logEvent =>
                {
                    JObject json = _converter.Value.GetGelfJson(logEvent);
                    Task resultTask = _transport.Value.Send(json.ToString(Newtonsoft.Json.Formatting.None));
                    return resultTask;
                }).ToArray();

                Task.WaitAll(sendTasks);
                base.EmitBatch(events);
            }
            catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
            }
            // ReSharper disable once PossibleMultipleEnumeration
        }

        protected override Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            try
            {
                IEnumerable<Task> sendTasks = events.Select(logEvent =>
                {
                    JObject json = _converter.Value.GetGelfJson(logEvent);
                    Task resultTask = _transport.Value.Send(json.ToString(Newtonsoft.Json.Formatting.None));
                    return resultTask;
                });

                return Task.WhenAll(sendTasks);
            }
            catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
                return Task.CompletedTask;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _transport.Value?.Dispose();
        }
    }
}