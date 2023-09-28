using System.Net;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport
{
    using System.Linq;
    using System.Net.Sockets;

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
        Task<IPAddress?> GetIpAddress(string hostNameOrAddress);
    }

    public class DnsWrapper : IDnsInfoProvider
    {
        /// <summary>
        /// Gets the host addresses.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or address.</param>
        /// <returns></returns>
        /// <exception cref="System.Net.Sockets.SocketException">When resolve <paramref name="hostNameOrAddress" /> trows exception.</exception>
        public Task<IPAddress[]> GetHostAddresses(string hostNameOrAddress)
        {
            return Dns.GetHostAddressesAsync(hostNameOrAddress);
        }

        public async Task<IPAddress?> GetIpAddress(string hostNameOrAddress)
        {
            if (string.IsNullOrEmpty(hostNameOrAddress))
            {
                return default;
            }

            var addresses = await GetHostAddresses(hostNameOrAddress).ConfigureAwait(false);
            var result = addresses.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
            return result;
        }
    }
}
