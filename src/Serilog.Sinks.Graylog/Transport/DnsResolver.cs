using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Serilog.Sinks.Graylog.Transport
{
    /// <summary>
    /// Base class for resolve dns
    /// </summary>
    public interface IDnsInfoProvider
    {
        /// <summary>
        /// Gets the host addresses.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or address.</param>
        /// <returns></returns>
        IPAddress[] GetHostAddresses(string hostNameOrAddress);
    }

    public class DnsWrapper : IDnsInfoProvider
    {
        /// <summary>
        /// Gets the host addresses.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or address.</param>
        /// <returns></returns>
        /// <exception cref="SocketException">When resolve <paramref name="hostNameOrAddress" /> trows exception.</exception>
        public IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            return Dns.GetHostAddresses(hostNameOrAddress);
        }
    }
}