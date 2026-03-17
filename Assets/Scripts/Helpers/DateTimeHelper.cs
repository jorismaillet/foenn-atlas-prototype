using System;
using System.Globalization;

namespace Assets.Scripts.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime ToDateTime(string yyyyMMddHH) => DateTime.SpecifyKind(DateTime.ParseExact(yyyyMMddHH, "yyyyMMddHH", CultureInfo.InvariantCulture),
            DateTimeKind.Utc
        );

        public static string ToTimestamp(string yyyyMMddHH) => new DateTimeOffset(ToDateTime(yyyyMMddHH)).ToUnixTimeSeconds().ToString();

        public static DateTime FromTimestamp(string timestamp) => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timestamp)).UtcDateTime;

    }
}
