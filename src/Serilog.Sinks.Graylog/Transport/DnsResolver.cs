using System.Net;
using System.Net.Sockets;

namespace Serilog.Sinks.Graylog.Transport
{
    /// <summary>
    /// Base class for resolve dns
    /// </summary>
    public abstract class DnsBase
    {
        /// <summary>
        /// Gets the host addresses.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or address.</param>
        /// <returns></returns>
        public abstract IPAddress[] GetHostAddresses(string hostNameOrAddress);
    }

    public class DnsWrapper : DnsBase
    {
        /// <summary>
        /// Gets the host addresses.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or address.</param>
        /// <returns></returns>
        /// <remarks>Fucking russian locale</remarks>
        /// <exception cref="SocketException">При разрешении <paramref name="hostNameOrAddress" /> возникает ошибка.</exception>
        public override IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            return Dns.GetHostAddresses(hostNameOrAddress);
        }
    }
}