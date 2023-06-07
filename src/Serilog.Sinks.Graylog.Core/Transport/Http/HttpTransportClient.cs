using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Serilog.Debugging;

namespace Serilog.Sinks.Graylog.Core.Transport.Http
{
    public class HttpTransportClient : ITransportClient<string>
    {
        private readonly string _graylogUrl;
        private readonly HttpClient _httpClient;

        public HttpTransportClient(string graylogUrl, AuthenticationHeaderValue authorization = null)
        {
            _graylogUrl = graylogUrl;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

            if (authorization != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = authorization;
            }
        }

        public async Task Send(string message)
        {
            var content = new StringContent(message, System.Text.Encoding.UTF8, "application/json");
            var url = new Uri(_graylogUrl);

            HttpResponseMessage result = await _httpClient.PostAsync(url, content).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                SelfLog.WriteLine("Unable send log message to graylog via HTTP transport");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}