using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Batching
{
    public class PeriodicBatchingGraylogSink : IBatchedLogEventSink
    {
        private readonly Lazy<ITransport> _transport;
        private readonly Lazy<IGelfConverter> _converter;

        public PeriodicBatchingGraylogSink(BatchingGraylogSinkOptions options)
        {
            ISinkComponentsBuilder sinkComponentsBuilder = new SinkComponentsBuilder(options);
            _transport = new Lazy<ITransport>(sinkComponentsBuilder.MakeTransport);
            _converter = new Lazy<IGelfConverter>(sinkComponentsBuilder.MakeGelfConverter);
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }

        Task IBatchedLogEventSink.EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            try
            {
                IEnumerable<Task> sendTasks = batch.Select(async logEvent =>
                {
                    JsonObject json = _converter.Value.GetGelfJson(logEvent);

                    await _transport.Value.Send(json.ToString());
                });

                return Task.WhenAll(sendTasks);
            } catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
                return Task.CompletedTask;
            }
        }
    }
}
