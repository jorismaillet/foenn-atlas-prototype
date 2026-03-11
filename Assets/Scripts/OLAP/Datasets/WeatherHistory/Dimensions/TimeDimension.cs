using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class TimeDimension : IDimension
    {
        public string TableName => "time_dimension";

        public Field PrimaryKey => Field.PK();

        public static Field timestamp = Field.Int64("timestamp");
        public static Field year = Field.Int16("year");
        public static Field month = Field.Int16("month");
        public static Field day = Field.Int16("day");
        public static Field hour = Field.Int16("hour");
        public static Field duration = Field.Int16("duration");

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

        public static DateTime ToDateTime(string yyyyMMddhh) => DateTime.ParseExact(yyyyMMddhh, "yyyyMMddHH", CultureInfo.InvariantCulture);

        public static string ToTimestamp(string yyyyMMddHH) => new DateTimeOffset(ToDateTime(yyyyMMddHH)).ToUnixTimeSeconds().ToString();
    }
}
