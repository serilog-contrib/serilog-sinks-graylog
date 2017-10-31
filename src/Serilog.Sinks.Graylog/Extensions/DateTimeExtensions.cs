using System;

namespace Serilog.Sinks.Graylog.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts to nix date time.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static double ConvertToNix(this DateTime dateTime)
        {
            var duration = dateTime.ToUniversalTime() - Epoch;
            return Math.Round(duration.TotalSeconds, 3, MidpointRounding.AwayFromZero);
        }

    }
}