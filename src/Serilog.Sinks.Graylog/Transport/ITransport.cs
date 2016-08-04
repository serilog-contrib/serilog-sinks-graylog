namespace Serilog.Sinks.Graylog.Transport
{
    /// <summary>
    /// The Transport interface
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Sends the specified target.
        /// </summary>
        /// <param name="message">The message.</param>
        void Send(string message);
    }
}