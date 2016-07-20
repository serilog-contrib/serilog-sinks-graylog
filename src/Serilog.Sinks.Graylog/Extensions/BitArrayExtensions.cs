using System.Collections;

namespace Serilog.Sinks.Graylog.Extensions
{
    public static class BitArrayExtensions
    {
        /// <summary>
        /// Inserts bits from the given byte into the given BitArray instance.
        /// </summary>
        /// <param name="bitArray">BitArray instance to be populated with bits</param>
        /// <param name="bitArrayIndex">Index pointer in BitArray to start inserting bits from</param>
        /// <param name="byteData">Byte to extract bits from and insert into the given BitArray instance</param>
        /// <param name="byteDataIndex">Index pointer in byteData to start extracting bits from</param>
        /// <param name="length">Number of bits to extract from byteData</param>
        public static void AddToBitArray(this BitArray bitArray, int bitArrayIndex, byte byteData, int byteDataIndex, int length)
        {
            var localBitArray = new BitArray(new[] { byteData });

            for (int i = byteDataIndex + length - 1; i >= byteDataIndex; i--)
            {
                bitArray.Set(bitArrayIndex, localBitArray.Get(i));
                bitArrayIndex++;
            }
        }
    }
}