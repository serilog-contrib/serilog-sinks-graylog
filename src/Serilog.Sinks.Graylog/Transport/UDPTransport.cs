using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Serilog.Sinks.Graylog.Extensions;

namespace Serilog.Sinks.Graylog.Transport
{
    /// <summary>
    /// <seealso cref="http://docs.graylog.org/en/2.0/pages/gelf.html"/>
    /// </summary>
    public class UDPTransport : ITransport
    {
        /// <summary>
        /// The transport client
        /// </summary>
        private readonly ITransportClient _transportClient;

        /// <summary>
        /// The maximum message size in UDP
        /// <remarks>
        /// UDP datagrams are usually limited to a size of 8192 bytes
        /// </remarks>
        /// </summary>
        private const int MaxMessageSizeInUdp = 8192;

        /// <summary>
        /// The prerfix size
        /// </summary>
        private const int PrerfixSize = 12;

        /// <summary>
        /// The maximum message size in chunk
        /// </summary>
        private const int MaxMessageSizeInChunk = MaxMessageSizeInUdp - PrerfixSize;

        /// <summary>
        /// The maximum number of chunks allowed
        /// </summary>
        private const int MaxNumberOfChunksAllowed = 128;

        /// <summary>
        /// Initializes a new instance of the <see cref="UDPTransport"/> class.
        /// </summary>
        /// <param name="transportClient">The transport client.</param>
        public UDPTransport(ITransportClient transportClient)
        {
            _transportClient = transportClient;
        }


        /// <exception cref="ArgumentException">message was too long</exception>
        /// <exception cref="ArgumentNullException">Свойство <paramref name="array" /> имеет значение null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Значение параметра <paramref name="index" /> меньше значения нижней границы массива <paramref name="array" />.</exception>
        /// <exception cref="ArrayTypeMismatchException">Тип исходной коллекции <see cref="T:System.Array" /> нельзя автоматически привести к типу массива назначения <paramref name="array" />.</exception>
        /// <exception cref="RankException">Исходный массив является многомерным.</exception>
        /// <exception cref="InvalidCastException">По меньшей мере, один элемент исходного массива <see cref="T:System.Array" /> не удается привести к типу массива назначения <paramref name="array" />.</exception>
        /// <exception cref="OverflowException">Массив является многомерным и содержит более <see cref="F:System.Int32.MaxValue" /> элементов.</exception>
        public void Send(IPEndPoint target, string message)
        {
            IPEndPoint ipEndPoint = target;
            byte[] compressedMessage = Compress(message);
            int messageLength = compressedMessage.Length;

            if (compressedMessage.Length > MaxMessageSizeInUdp)
            {
                int chunksCount = messageLength / MaxMessageSizeInChunk + 1;

                if (chunksCount > MaxNumberOfChunksAllowed)
                {
                    throw new ArgumentException("message was too long", nameof(message));
                }
                byte[] messageId = GenerateMessageId(compressedMessage);
                for (var i = 0; i < chunksCount; i++)
                {
                    int skip = i * MaxMessageSizeInChunk;
                    byte[] chunkHeader = ConstructChunkHeader(messageId, i, chunksCount);
                    byte[] chunkData = compressedMessage.Skip(skip).Take(MaxMessageSizeInChunk).ToArray();

                    var messageChunkFull = new byte[chunkHeader.Length + chunkData.Length];
                    chunkHeader.CopyTo(messageChunkFull, 0);
                    chunkData.CopyTo(messageChunkFull, chunkHeader.Length);
                    _transportClient.Send(messageChunkFull, messageChunkFull.Length, ipEndPoint);
                }
            }
            else
            {
                _transportClient.Send(compressedMessage, messageLength, target);
            }
        }

        private static byte[] ConstructChunkHeader(byte[] messageId, int chunkNumber, int chunksCount)
        {
            var result = new byte[12];
            result[0] = 0x1e;
            result[1] = 0x0f;
            messageId.CopyTo(result, 2);
            result[10] = (byte)chunkNumber;
            result[11] = (byte)chunksCount;
            return result;
        }

        /// <summary>
        /// Sends the specified server ip address.
        /// </summary>
        /// <param name="serverIpAddress">The server ip address.</param>
        /// <param name="serverPort">The server port.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException">Параметр <paramref name="address" /> имеет значение null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Значение свойства <paramref name="port" /> меньше значения <see cref="F:System.Net.IPEndPoint.MinPort" />.– или – Значение <paramref name="port" /> больше значения <see cref="F:System.Net.IPEndPoint.MaxPort" />.– или – <paramref name="address" /> меньше 0 или больше 0x00000000FFFFFFFF.</exception>
        public void Send(string serverIpAddress, int serverPort, string message)
        {
            IPAddress ipAddress = IPAddress.Parse(serverIpAddress);
            var ipEndPoint = new IPEndPoint(ipAddress, serverPort);

            Send(ipEndPoint, message);
        }

        /// <summary>
        /// Compresses the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>compressed message to array of bytes</returns>
        private static byte[] Compress(string message)
        {
            var resultStream = new MemoryStream();
            using (var gzipStream = new GZipStream(resultStream, CompressionMode.Compress))
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                gzipStream.Write(messageBytes, 0, messageBytes.Length);
            }
            return resultStream.ToArray();
        }

        /// <summary>
        /// Generates the message identifier.
        /// </summary>
        /// <param name="compressedMessage">The compressed message.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Can't get ip adress from dns</exception>
        private static byte[] GenerateMessageId(byte[] compressedMessage)
        {
            
            IPAddress ipAddress = Dns.GetHostAddresses(Dns.GetHostName())
                .FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork);
            if (ipAddress == null)
            {
                throw new InvalidOperationException("Can't get ip adress from dns");
            }
            //read bytes of the last 2 segments and insert bits into the bit array
            var bitArray = new BitArray(64);
            byte[] addressBytes = ipAddress.GetAddressBytes();
            bitArray.AddToBitArray(0, addressBytes[2], 0, 8);
            bitArray.AddToBitArray(8, addressBytes[3], 0, 8);

            int second = DateTime.Now.Second;
            bitArray.AddToBitArray(16, (byte)second, 0, 6);
            byte[] messageHash;
            using (var md5 = MD5.Create())
            {
                messageHash = md5.ComputeHash(compressedMessage);
            }
            var startIndex = 22;

            for (var hashByteIndex = 0; hashByteIndex < 5; hashByteIndex++)
            {
                var hashByte = messageHash[hashByteIndex];
                bitArray.AddToBitArray(startIndex, hashByte, 0, 8);
                startIndex += 8;
            }
            var result = new byte[8];
            bitArray.CopyTo(result, 0);
            return result;
        }
    }
}