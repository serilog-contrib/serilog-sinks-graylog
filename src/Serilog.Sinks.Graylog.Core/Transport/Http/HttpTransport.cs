using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Http
{
    public class HttpTransport : ITransport
    {
        private readonly ITransportClient<string> _transportClient;

        public HttpTransport(ITransportClient<string> transportClient)
        {
            _transportClient = transportClient;
        }

        public Task Send(string message)
        {
            return _transportClient.Send(message);
        }
    }
}