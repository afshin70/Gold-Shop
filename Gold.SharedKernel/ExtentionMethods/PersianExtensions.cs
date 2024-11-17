using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Gold.SharedKernel.ExtentionMethods
{
    public static class PersianExtensions
    {
        /// <summary>
        /// Get fa-IR Date
        /// </summary>
        /// <param name="utcDateTimeOffset"></param>
        /// <returns></returns>
        public static string ToPersianDateTimeString(this DateTimeOffset utcDateTimeOffset, string format = "f")
        {
            if (utcDateTimeOffset <= DateTimeOffset.MinValue) return "";
            TimeZoneInfo iranTimeZone;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                iranTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            }
            else //RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            {
                iranTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran");
            }
            var localDateTimeFromUtc = TimeZoneInfo.ConvertTimeFromUtc(utcDateTimeOffset.DateTime, iranTimeZone);
            var iranCultureInfo = new CultureInfo("fa-IR");
            return localDateTimeFromUtc.ToString(format, iranCultureInfo.DateTimeFormat).ToPersianNumbers();
        }

        /// <summary>
        /// Get Persian Numbers
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToPersianNumbers(this string s)
        {
            return s.Replace("0", "۰").Replace("1", "۱").Replace("2", "۲").Replace("3", "۳").Replace("4", "۴").Replace("5", "۵").Replace("6", "۶").Replace("7", "۷").Replace("8", "۸").Replace("9", "۹");
        }
    }
}
