using Serilog.Debugging;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace Serilog.Sinks.Graylog.Core.Helpers
{
    public class HttpBasicAuthenticationGenerator
    {
        private readonly string? _usernameInHttp;
        private readonly string? _passwordInHttp;

        public HttpBasicAuthenticationGenerator(string? usernameInHttp, string? passwordInHttp)
        {
            _usernameInHttp = usernameInHttp;
            _passwordInHttp = passwordInHttp;
        }

        public AuthenticationHeaderValue? Generate()
        {
            if (!Validate()) return null;

            var byteArray = Encoding.ASCII.GetBytes(string.Concat(_usernameInHttp, ":", _passwordInHttp));

            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        private bool Validate()
        {
            if (_usernameInHttp == null && _passwordInHttp == null) return false;

            if (_passwordInHttp == null)
            {
                SelfLog.WriteLine("UsernameInHttp has value, but passwordInHttp is empty. Therefore basic authentication not used");
                return false;
            }

            if (_usernameInHttp == null)
            {
                SelfLog.WriteLine("PasswordInHttp has value, but UsernameInHttp is empty. Therefore basic authentication not used");
                return false;
            }

            return true;
        }
    }
}
