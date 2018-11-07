using System;

namespace Serilog.Sinks.Graylog.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts to nix date time.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static double ConvertToNix(this DateTimeOffset dateTimeOffset)
        {
            var duration = dateTimeOffset.ToUniversalTime() - Epoch;
            return Math.Round((double) duration.TotalSeconds, 3, MidpointRounding.AwayFromZero);
        }

    }
}