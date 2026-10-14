using System;
using System.Security;
using Orckestra.Composer.Logging;

namespace Orckestra.Composer.Utils
{
    /// <summary>
    /// Resolves a store's Windows time zone without throwing when the time zone id is empty or unknown
    /// (typo, IANA name, or a Windows time zone missing on a server without the latest updates).
    /// </summary>
    public static class StoreTimeZoneHelper
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Returns the store's time zone, or null when the time zone id is empty or cannot be resolved.
        /// A warning with the store number and time zone value is logged when the id cannot be resolved.
        /// </summary>
        public static TimeZoneInfo GetStoreTimeZone(string timeZoneId, string storeNumber)
        {
            if (string.IsNullOrWhiteSpace(timeZoneId)) { return null; }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException
                                       || ex is InvalidTimeZoneException
                                       || ex is SecurityException)
            {
                Log.WarnException("Store {0} has an invalid time zone '{1}'. Server time is used instead.", ex, storeNumber, timeZoneId);
                return null;
            }
        }

        /// <summary>
        /// Returns the current date and time in the store's time zone,
        /// or the server's local time when the store's time zone is empty or cannot be resolved.
        /// </summary>
        public static DateTime GetStoreNow(string timeZoneId, string storeNumber)
        {
            return GetStoreNow(timeZoneId, storeNumber, DateTime.UtcNow);
        }

        /// <summary>
        /// Converts <paramref name="utcNow"/> to the store's time zone,
        /// or to the server's local time when the store's time zone is empty or cannot be resolved.
        /// </summary>
        public static DateTime GetStoreNow(string timeZoneId, string storeNumber, DateTime utcNow)
        {
            var utc = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
            var storeTimeZone = GetStoreTimeZone(timeZoneId, storeNumber);

            return storeTimeZone != null
                ? TimeZoneInfo.ConvertTimeFromUtc(utc, storeTimeZone)
                : utc.ToLocalTime();
        }
    }
}
