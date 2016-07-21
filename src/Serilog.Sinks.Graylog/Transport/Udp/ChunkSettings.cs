using Serilog.Sinks.Graylog.Helpers;

namespace Serilog.Sinks.Graylog.Transport.Udp
{
    public class ChunkSettings
    {
        public MessageIdGeneratortype MessageIdGeneratorType { get; set; }

        /// <summary>
        /// The prefix size
        /// <seealso cref="http://docs.graylog.org/en/2.0/pages/gelf.html"/>
        /// </summary>
        public const byte PrefixSize = 12;

        /// <summary>
        /// The message identifier size
        /// <seealso cref="http://docs.graylog.org/en/2.0/pages/gelf.html"/>
        /// </summary>
        public const byte MessageIdSize = 16;

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
        public const int MaxMessageSizeInUdp = 8192;

        /// <summary>
        /// The maximum message size in chunk
        /// </summary>
        public const int MaxMessageSizeInChunk = MaxMessageSizeInUdp - PrefixSize;
    }
}