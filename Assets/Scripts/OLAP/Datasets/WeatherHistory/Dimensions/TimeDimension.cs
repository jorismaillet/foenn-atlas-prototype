using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class TimeDimension : IDimension
    {
        public string name => "time_dimension";

        public Field PrimaryKey => Field.PK();

        public static Field timestamp = Field.IntAttribute("timestamp");
        public static Field year = Field.IntAttribute("year");
        public static Field month = Field.IntAttribute("month");
        public static Field day = Field.IntAttribute("day");
        public static Field hour = Field.IntAttribute("hour");
        public static Field duration = Field.IntAttribute("duration");

        public List<IndexDefinition> Indexes => new List<IndexDefinition>()
        {
        };

        private static SourceAttribute AAAAMMJJHH = new SourceAttribute("AAAAMMJJHH", SourceAttributeType.String);

        public List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Compute(AAAAMMJJHH, timestamp, ToTimestamp),
            FieldMap.Compute(AAAAMMJJHH, year, s => s.Substring(0, 4)),
            FieldMap.Compute(AAAAMMJJHH, month, s => s.Substring(4, 2)),
            FieldMap.Compute(AAAAMMJJHH, day, s => s.Substring(6, 2)),
            FieldMap.Compute(AAAAMMJJHH, hour, s => s.Substring(8, 2)),
            FieldMap.Compute(AAAAMMJJHH, duration, s => "1")
        };

        public static DateTime ToDateTime(string yyyyMMddHH) => DateTime.SpecifyKind(DateTime.ParseExact(yyyyMMddHH, "yyyyMMddHH", CultureInfo.InvariantCulture),
            DateTimeKind.Utc
        );

        public static string ToTimestamp(string yyyyMMddHH) => new DateTimeOffset(ToDateTime(yyyyMMddHH)).ToUnixTimeSeconds().ToString();

        public static DateTime FromTimestamp(string timestamp) => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timestamp)).UtcDateTime;
    }
}
