using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Serilog.Debugging;

namespace Serilog.Sinks.Graylog.Transport.Http
{
    public class HttpTransportClient : ITransportClient<string>
    {
        private readonly string _graylogUrl;
        private readonly HttpClient _httpClient;

        public HttpTransportClient(string graylogUrl)
        {
            _graylogUrl = graylogUrl;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }

        public Task Send(string message)
        {
            var content = new StringContent(message, System.Text.Encoding.UTF8, "application/json");
            var url = new Uri(_graylogUrl);

            try
            {
                var result = _httpClient.PostAsync(url, content).Result;
                if (!result.IsSuccessStatusCode)
                {
                    throw new LoggingFailedException("Unable send log message to graylog via httptransporrt");
                }
            }
            catch (Exception e)
            {
                throw new LoggingFailedException("Unable send log message to graylog via httptransporrt");
            }
            return Task.CompletedTask;
        }
    }
}