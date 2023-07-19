using Serilog.Debugging;
using Serilog.Sinks.Graylog.Core.Helpers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Http
{
    public class HttpTransportClient : ITransportClient<string>
    {
        private const string _defaultHttpUriPath = "/gelf";

        private HttpClient? _httpClient;

        private readonly GraylogSinkOptionsBase options;

        public HttpTransportClient(GraylogSinkOptionsBase options)
        {
            this.options = options;
        }

        protected virtual HttpClient CreateHttpClient() => new();

        protected virtual void ConfigureHttpClient(HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(options.HostnameOrAddress))
            {
                throw new InvalidOperationException("The HostnameOrAddress value must be set.");
            }

            var builder = new UriBuilder(options.HostnameOrAddress)
            {
                Port = options.Port.GetValueOrDefault(443)
            };

            if (options.UseSsl)
            {
                builder.Scheme = "https";
            }

            httpClient.BaseAddress = builder.Uri;

            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

            var authenticationHeaderValue = new HttpBasicAuthenticationGenerator(options.UsernameInHttp, options.PasswordInHttp).Generate();

            if (authenticationHeaderValue != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
            }
        }

        private void EnsureHttpClient()
        {
            if (_httpClient == null)
            {
                _httpClient = CreateHttpClient();

                ConfigureHttpClient(_httpClient);
            }
        }

        public async Task Send(string message)
        {
            EnsureHttpClient();

            var content = new StringContent(message, Encoding.UTF8, "application/json");

            HttpResponseMessage result = await _httpClient!.PostAsync(_defaultHttpUriPath, content).ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                SelfLog.WriteLine("Unable send log message to graylog via HTTP transport");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
        }
    }
}
