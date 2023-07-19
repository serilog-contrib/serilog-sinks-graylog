using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Serilog.Sinks.Graylog.Core.Helpers
{
    public interface IMessageIdGenerator
    {
        /// <summary>
        /// Generates the message identifier.
        /// </summary>
        /// <returns></returns>
        byte[] GenerateMessageId(byte[] message);
    }

    public enum MessageIdGeneratorType
    {
        Timestamp,
        Md5
    }

    /// <summary>
    /// Generate message id from new GUID
    /// </summary>
    /// <seealso cref="IMessageIdGenerator" />
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
    /// <seealso cref="IMessageIdGenerator" />
    public sealed class Md5MessageIdGenerator : IMessageIdGenerator
    {
        public byte[] GenerateMessageId(byte[] message)
        {
#if NETSTANDARD2_0
            using MD5 md5 = MD5.Create();
            
            byte[] messageHash = md5.ComputeHash(message);
#else
            byte[] messageHash = MD5.HashData(message);
#endif

            return messageHash.Take(8).ToArray();
        }
    }

    public interface IMessageIdGeneratorResolver
    {
        IMessageIdGenerator Resolve(MessageIdGeneratorType generatorType);
    }

    public sealed class MessageIdGeneratorResolver : IMessageIdGeneratorResolver
    {
        private readonly Dictionary<MessageIdGeneratorType, Lazy<IMessageIdGenerator>> _messageGenerators = new()
        {
            [MessageIdGeneratorType.Timestamp] = new Lazy<IMessageIdGenerator>(() => new TimestampMessageIdGenerator()),
            [MessageIdGeneratorType.Md5] = new Lazy<IMessageIdGenerator>(() => new Md5MessageIdGenerator())
        };

        /// <exception cref="ArgumentOutOfRangeException">Condition.</exception>
        public IMessageIdGenerator Resolve(MessageIdGeneratorType generatorType)
        {
            return _messageGenerators[generatorType].Value;
        }
    }
}
