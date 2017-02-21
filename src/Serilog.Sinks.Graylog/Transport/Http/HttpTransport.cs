using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Serilog.Sinks.Graylog.Transport.Http
{
    public class HttpTransport : ITransport
    {
        private readonly string _serverUrl;
        private readonly HttpClient _httpClient;

        public HttpTransport(string serverUrl)
        {
            _serverUrl = serverUrl;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }

        public void Send(string message)
        {
            var content = new StringContent(message, System.Text.Encoding.UTF8, "application/json");

            var url = new Uri(_serverUrl);
            var result = _httpClient.PostAsync(url, content).GetAwaiter().GetResult();
            if (!result.IsSuccessStatusCode)
            {
                Trace.WriteLine($"Unable to send log message: {result.ReasonPhrase}", "GELF");
            }
        }
    }
}
