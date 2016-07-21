using System;
using System.Linq;
using System.Security.Cryptography;

namespace Serilog.Sinks.Graylog.Helpers
{
    public interface IMessageIdGenerator
    {
        byte[] GenerateMessageId();
    }

    public enum MessageIdGeneratortype
    {
        Timestamp,
        Md5
    }

    /// <summary>
    /// Generate message id from new GUID
    /// </summary>
    /// <seealso cref="Serilog.Sinks.Graylog.Helpers.IMessageIdGenerator" />
    public class TimestampMessageIdGenerator : IMessageIdGenerator
    {
        public byte[] GenerateMessageId()
        {
            return BitConverter.GetBytes(DateTime.Now.Ticks);
        }
    }

    /// <summary>
    /// Generate message Id from first 8 bytes of MD5 hash
    /// </summary>
    /// <seealso cref="Serilog.Sinks.Graylog.Helpers.IMessageIdGenerator" />
    public class Md5MessageIdGenerator : IMessageIdGenerator
    {
        private readonly byte[] _message;

        public Md5MessageIdGenerator(byte[] message)
        {
            _message = message;
        }

        public byte[] GenerateMessageId()
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] messageHash = md5.ComputeHash(_message);
                return messageHash.Take(8).ToArray();
            }
        }
    }
}