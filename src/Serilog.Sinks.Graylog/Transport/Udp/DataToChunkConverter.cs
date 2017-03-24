using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Sinks.Graylog.Helpers;

namespace Serilog.Sinks.Graylog.Transport.Udp
{
    /// <summary>
    /// Converts Data to udp chunks
    /// </summary>
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

    /// <inheritdoc/>
    public sealed class DataToChunkConverter : IDataToChunkConverter
    {
        private readonly ChunkSettings _settings;
        private readonly IMessageIdGeneratorResolver _generatorResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataToChunkConverter"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="generatorResolver">The generator resolver.</param>
        public DataToChunkConverter(ChunkSettings settings, IMessageIdGeneratorResolver generatorResolver)
        {
            _settings = settings;
            _generatorResolver = generatorResolver;
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

            //TOD: FixMe
            IMessageIdGenerator messageIdGenerator = _generatorResolver.Resolve(_settings.MessageIdGeneratorType, message);
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

        private static IList<byte> ConstructChunkHeader(byte[] messageId, byte chunkNumber, byte chunksCount)
        {
            var result = new List<byte>(ChunkSettings.PrefixSize);
            result.AddRange(ChunkSettings.GelfMagicBytes);
            result.AddRange(messageId);
            result.Add(chunkNumber);
            result.Add(chunksCount);
            return result;
        }
    }
}