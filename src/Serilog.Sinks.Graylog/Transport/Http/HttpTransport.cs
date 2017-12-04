using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Serilog.Debugging;

namespace Serilog.Sinks.Graylog.Transport.Http
{
    public sealed class HttpTransport : ITransport
    {
        private readonly Uri graylogUrl;
        private readonly HttpClient httpClient;

        public HttpTransport(Uri graylogUrl)
        {
            this.graylogUrl = graylogUrl;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }

        public async Task Send(string message)
        {
            var content = new StringContent(message, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(graylogUrl, content).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
            {
                throw new LoggingFailedException("Unable send log message to graylog via HTTP transport");
            }
        }

        public void Dispose() => httpClient.Dispose();
    }
}