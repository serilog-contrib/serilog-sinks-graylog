using System;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport
{
    /// <summary>
    /// The Transport client interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITransportClient<in T> : IDisposable
    {
        /// <summary>
        /// Sends the specified payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        Task Send(T payload);
    }
}
