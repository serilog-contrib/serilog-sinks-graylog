using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Serilog.Sinks.Graylog.Core.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ToGzip(this string source)
        {
            var resultStream = new MemoryStream();

            using (var gzipStream = new GZipStream(resultStream, CompressionMode.Compress))
            {
                byte[] messageBytes = ToByteArray(source);

                gzipStream.Write(messageBytes, 0, messageBytes.Length);
            }

            return resultStream.ToArray();
        }

        public static byte[] ToByteArray(this string source) => Encoding.UTF8.GetBytes(source);

        /// <summary>
        /// Truncates the specified maximum length.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns></returns>
        public static string Truncate(this string source, int maxLength)
        {
            return source.Length > maxLength ? source.Substring(0, maxLength) : source;
        }

        public static string Expand(this string source)
        {
            return Environment.ExpandEnvironmentVariables(source);
        }
    }
}
