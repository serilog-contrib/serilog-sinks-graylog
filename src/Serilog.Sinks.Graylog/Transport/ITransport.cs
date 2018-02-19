using System;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Transport
{
    /// <summary>
    /// The Transport interface
    /// </summary>
    public interface ITransport: IDisposable
    {
        /// <summary>
        /// Sends the specified target asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        Task SendAsync(string message);
    }
}