using System.IO;
using System.IO.Compression;
using System.Text;

namespace Serilog.Sinks.Graylog.Extensions
{
    public static class StringExtensions
    {
        public static byte[] Compress(this string source)
        {
            var resultStream = new MemoryStream();
            using (var gzipStream = new GZipStream(resultStream, CompressionMode.Compress))
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(source);
                gzipStream.Write(messageBytes, 0, messageBytes.Length);
            }
            return resultStream.ToArray();
        }
    }
}