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
        private readonly Func<HttpClient> factory;
        private HttpClient httpClient;

        public HttpTransport(Uri graylogUrl, Func<HttpClient> factory = null)
        {
            this.graylogUrl = graylogUrl;
            this.factory = factory;
        }

        public async Task SendAsync(string message)
        {
            // Initialize the client if it is new and create a message payload
            InitClient();
            var content = new StringContent(message, Encoding.UTF8, "application/json");

            // Attempt send
            HttpResponseMessage result;
            try
            {
                result = await httpClient.PostAsync(graylogUrl, content).ConfigureAwait(false);
                SelfLog.WriteLine($"Sent HTTP Gelf Message");
            }
            catch (Exception exc)
            {
                SelfLog.WriteLine("Error sending HTTP message", exc);
                Dispose();
                throw;
            }

            // Validate result
            if (!result.IsSuccessStatusCode)
            {
                const string error = "Unable send log message to graylog via HTTP transport";
                SelfLog.WriteLine(error);
                throw new LoggingFailedException(error);
            }
        }

        private void InitClient()
        {
            if (httpClient == null)
            {
                httpClient = factory?.Invoke() ?? new HttpClient();
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            }
        }

        public void Dispose()
        {
            if (factory != null) return;
            httpClient.Dispose();
        }
    }
}