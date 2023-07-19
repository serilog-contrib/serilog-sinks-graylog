using System;

namespace Serilog.Sinks.Graylog.Core.Extensions
{
    /// <summary>
    /// Some type extensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether [is numeric type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is numeric type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumericType(this Type type)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte or
                TypeCode.SByte or
                TypeCode.UInt16 or
                TypeCode.UInt32 or
                TypeCode.UInt64 or
                TypeCode.Int16 or
                TypeCode.Int32 or
                TypeCode.Int64 or
                TypeCode.Decimal or
                TypeCode.Double or
                TypeCode.Single => true,
                _ => false,
            };
        }
    }
}
