using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

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
        Task<IPAddress[]> GetHostAddresses(string hostNameOrAddress);
    }

    public class DnsWrapper : IDnsInfoProvider
    {
        /// <summary>
        /// Gets the host addresses.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or address.</param>
        /// <returns></returns>
        /// <exception cref="SocketException">When resolve <paramref name="hostNameOrAddress" /> trows exception.</exception>
        public async Task<IPAddress[]> GetHostAddresses(string hostNameOrAddress)
        {
            return await Dns.GetHostAddressesAsync(hostNameOrAddress);
        }
    }
}