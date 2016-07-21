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
    public sealed class TimestampMessageIdGenerator : IMessageIdGenerator
    {
        private readonly DateTime _dateToGenerateId;

        public TimestampMessageIdGenerator(DateTime dateToGenerateId)
        {
            _dateToGenerateId = dateToGenerateId;
        }

        public byte[] GenerateMessageId()
        {
            return BitConverter.GetBytes(_dateToGenerateId.Ticks);
        }
    }

    /// <summary>
    /// Generate message Id from first 8 bytes of MD5 hash
    /// </summary>
    /// <seealso cref="Serilog.Sinks.Graylog.Helpers.IMessageIdGenerator" />
    public sealed class Md5MessageIdGenerator : IMessageIdGenerator
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

    public interface IMessageIdGeneratorResolver
    {
        IMessageIdGenerator Resolve(MessageIdGeneratortype generatorType, byte[] message);
    }

    public sealed class MessageIdGeneratorResolver : IMessageIdGeneratorResolver
    {
        /// <exception cref="ArgumentOutOfRangeException">Condition.</exception>
        public IMessageIdGenerator Resolve(MessageIdGeneratortype generatorType, byte[] message)
        {
            switch (generatorType)
            {
                case MessageIdGeneratortype.Timestamp:
                    return new TimestampMessageIdGenerator(DateTime.Now);
                case MessageIdGeneratortype.Md5:
                    return new Md5MessageIdGenerator(message);
                default:
                    throw new ArgumentOutOfRangeException(nameof(generatorType), generatorType, null);
            }

        }
    }
}