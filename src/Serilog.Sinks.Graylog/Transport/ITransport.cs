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
        /// Sends the specified target.
        /// </summary>
        /// <param name="message">The message.</param>
        Task Send(string message);
    }
}