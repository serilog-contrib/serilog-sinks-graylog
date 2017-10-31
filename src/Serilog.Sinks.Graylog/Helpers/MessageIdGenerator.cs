using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Serilog.Sinks.Graylog.Helpers
{
    public interface IMessageIdGenerator
    {
        /// <summary>
        /// Generates the message identifier.
        /// </summary>
        /// <returns></returns>
        byte[] GenerateMessageId(byte[] message);
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
        public byte[] GenerateMessageId(byte[] message)
        {
            return BitConverter.GetBytes(DateTime.UtcNow.Ticks);
        }
    }

    /// <summary>
    /// Generate message Id from first 8 bytes of MD5 hash
    /// </summary>
    /// <seealso cref="Serilog.Sinks.Graylog.Helpers.IMessageIdGenerator" />
    public sealed class Md5MessageIdGenerator : IMessageIdGenerator
    {
        public byte[] GenerateMessageId(byte[] message)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] messageHash = md5.ComputeHash(message);
                return messageHash.Take(8).ToArray();
            }
        }
    }

    public interface IMessageIdGeneratorResolver
    {
        IMessageIdGenerator Resolve(MessageIdGeneratortype generatorType);
    }

    public sealed class MessageIdGeneratorResolver : IMessageIdGeneratorResolver
    {
        private Dictionary<MessageIdGeneratortype, Lazy<IMessageIdGenerator>> _messageGenerators = new Dictionary<MessageIdGeneratortype, Lazy<IMessageIdGenerator>>
        {
            [MessageIdGeneratortype.Timestamp] = new Lazy<IMessageIdGenerator>(() => new TimestampMessageIdGenerator()),
            [MessageIdGeneratortype.Md5] = new Lazy<IMessageIdGenerator>(() => new Md5MessageIdGenerator())
        };

        /// <exception cref="ArgumentOutOfRangeException">Condition.</exception>
        public IMessageIdGenerator Resolve(MessageIdGeneratortype generatorType)
        {
            return _messageGenerators[generatorType].Value;
        }
    }
}