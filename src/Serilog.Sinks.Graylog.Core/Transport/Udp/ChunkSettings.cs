using Serilog.Sinks.Graylog.Core.Helpers;

namespace Serilog.Sinks.Graylog.Core.Transport.Udp
{
    public sealed class ChunkSettings
    {
        /// <inheritdoc />
        public ChunkSettings(MessageIdGeneratorType messageIdGeneratorType, int maxMessageSizeInUdp)
        {
            MessageIdGeneratorType = messageIdGeneratorType;
            MaxMessageSizeInUdp = maxMessageSizeInUdp;
        }

        public MessageIdGeneratorType MessageIdGeneratorType { get; }

        /// <summary>
        /// The prefix size
        /// <seealso cref="http://docs.graylog.org/en/2.0/pages/gelf.html"/>
        /// </summary>
        public const byte PrefixSize = 12;

        /// <summary>
        /// The maximum number of chunks allowed
        /// <seealso cref="http://docs.graylog.org/en/2.0/pages/gelf.html"/>
        /// </summary>
        public const byte MaxNumberOfChunksAllowed = 128;

        /// <summary>
        /// The maximum message size in UDP
        /// <remarks>
        /// UDP chunks are usually limited to a size of 8192 bytes
        /// </remarks>
        /// </summary>
        public int MaxMessageSizeInUdp { get; }

        public static readonly byte[] GelfMagicBytes = { 0x1e, 0x0f };

        /// <summary>
        /// The maximum message size in chunk
        /// </summary>
        public int MaxMessageSizeInChunk => MaxMessageSizeInUdp - PrefixSize;
    }
}
