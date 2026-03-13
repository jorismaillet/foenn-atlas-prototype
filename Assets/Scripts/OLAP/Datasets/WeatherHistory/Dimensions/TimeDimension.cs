using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class TimeDimension : IDimension
    {
        public static string Name => "time_dimension";
        public string name => Name;

        public Field PrimaryKey => Field.PK(Name);
        public Field LookupField => yyyyMMddHH;
        public SourceAttribute LookupSourceAttribute => new SourceAttribute("AAAAMMJJHH", SourceAttributeType.String);

        public static Field yyyyMMddHH = Field.TextAttribute(Name, "yyyyMMddHH");
        public static Field timestamp = Field.IntAttribute(Name, "timestamp");
        public static Field year = Field.IntAttribute(Name, "year");
        public static Field month = Field.IntAttribute(Name, "month");
        public static Field day = Field.IntAttribute(Name, "day");
        public static Field hour = Field.IntAttribute(Name, "hour");
        public static Field duration = Field.IntAttribute(Name, "duration");

        public List<IndexDefinition> Indexes => new List<IndexDefinition>()
        {
            new IndexDefinition(true, LookupField),
            new IndexDefinition(false, year, month, day),
            new IndexDefinition(false, year, month),
            new IndexDefinition(false, year),
        };


        public List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(LookupSourceAttribute, yyyyMMddHH),
            FieldMap.Compute(LookupSourceAttribute, timestamp, ToTimestamp),
            FieldMap.Compute(LookupSourceAttribute, year, s => s.Substring(0, 4)),
            FieldMap.Compute(LookupSourceAttribute, month, s => s.Substring(4, 2)),
            FieldMap.Compute(LookupSourceAttribute, day, s => s.Substring(6, 2)),
            FieldMap.Compute(LookupSourceAttribute, hour, s => s.Substring(8, 2)),
            FieldMap.Compute(LookupSourceAttribute, duration, s => "1")
        };

        public static DateTime ToDateTime(string yyyyMMddHH) => DateTime.SpecifyKind(DateTime.ParseExact(yyyyMMddHH, "yyyyMMddHH", CultureInfo.InvariantCulture),
            DateTimeKind.Utc
        );

        public static string ToTimestamp(string yyyyMMddHH) => new DateTimeOffset(ToDateTime(yyyyMMddHH)).ToUnixTimeSeconds().ToString();

        public static DateTime FromTimestamp(string timestamp) => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timestamp)).UtcDateTime;
    }
}
