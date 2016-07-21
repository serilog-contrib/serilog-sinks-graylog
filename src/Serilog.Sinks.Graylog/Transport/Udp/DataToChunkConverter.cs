using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Sinks.Graylog.Helpers;

namespace Serilog.Sinks.Graylog.Transport.Udp
{
    public interface IDataToChunkConverter
    {
        /// <summary>
        /// Converts to chunks.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>array of chunks to save</returns>
        /// <exception cref="System.ArgumentException">message was too long</exception>
        /// <exception cref="ArgumentException">message was too long</exception>
        IList<byte[]> ConvertToChunks(byte[] message);
    }

    public sealed class DataToChunkConverter : IDataToChunkConverter
    {
        private readonly IDnsInfoProvider _dnsInfoProvider;
        private readonly ChunkSettings _settings;

        public DataToChunkConverter(IDnsInfoProvider dnsInfoProvider, ChunkSettings settings)
        {
            _dnsInfoProvider = dnsInfoProvider;
            _settings = settings;
        }

        /// <summary>
        /// Converts to chunks.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>array of chunks to save</returns>
        /// <exception cref="System.ArgumentException">message was too long</exception>
        /// <exception cref="ArgumentException">message was too long</exception>
        public IList<byte[]> ConvertToChunks(byte[] message)
        {
            int messageLength = message.Length;
            if (messageLength <= ChunkSettings.MaxMessageSizeInUdp)
            {
                return new List<byte[]>(1) {message};
            }

            int chunksCount = messageLength / ChunkSettings.MaxMessageSizeInChunk + 1;
            if (chunksCount > ChunkSettings.MaxNumberOfChunksAllowed)
            {
                throw new ArgumentException("message was too long", nameof(message));
            }

            IMessageIdGenerator messageIdGenerator = GetMessageIdGenerator(_settings.MessageIdGeneratorType, message);
            byte[] messageId = messageIdGenerator.GenerateMessageId();

            var result = new List<byte[]>();
            for (byte i = 0; i < chunksCount; i++)
            {
                IList<byte> chunkHeader = ConstructChunkHeader(messageId, i, (byte)chunksCount);

                int skip = i * ChunkSettings.MaxMessageSizeInChunk;
                byte[] chunkData = message.Skip(skip).Take(ChunkSettings.MaxMessageSizeInChunk).ToArray();

                var messageChunkFull = new List<byte>(chunkHeader.Count + chunkData.Length);
                messageChunkFull.AddRange(chunkHeader);
                messageChunkFull.AddRange(chunkData);
                result.Add(messageChunkFull.ToArray());
            }
            return result;
        }

        private IMessageIdGenerator GetMessageIdGenerator(MessageIdGeneratortype messageIdGeneratorType, byte[] message)
        {
            switch (messageIdGeneratorType)
            {
                case MessageIdGeneratortype.Timestamp:
                    return new TimestampMessageIdGenerator();
                case MessageIdGeneratortype.Md5:
                    return new Md5MessageIdGenerator(message);
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageIdGeneratorType), messageIdGeneratorType, null);
            }
        }

        private static IList<byte> ConstructChunkHeader(byte[] messageId, byte chunkNumber, byte chunksCount)
        {
            var result = new List<byte>(ChunkSettings.PrefixSize);
            result.Add(0x1e);
            result.Add(0x0f);
            result.AddRange(messageId);
            result.Add(chunkNumber);
            result.Add(chunksCount);
            return result;
        }
    }
}