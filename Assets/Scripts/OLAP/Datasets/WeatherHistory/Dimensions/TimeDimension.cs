using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class TimeDimension : Dimension
    {
        public override string Name { get; }
        public override Field PrimaryKey => Field.PK(Name);
        public override Field LookupField => yyyyMMddHH;
        public override SourceField LookupSourceAttribute { get; }

        public Field
            yyyyMMddHH,
            timestamp,
            year,
            month,
            day,
            hour,
            duration;

        public override List<IndexDefinition> Indexes => new List<IndexDefinition>()
        {
            new IndexDefinition(true, LookupField),
            new IndexDefinition(false, year, month, day),
            new IndexDefinition(false, year, month),
            new IndexDefinition(false, year),
        };

        public override List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(LookupSourceAttribute, yyyyMMddHH),
            FieldMap.Compute(LookupSourceAttribute, timestamp, ToTimestamp),
            FieldMap.Compute(LookupSourceAttribute, year, s => s.Substring(0, 4)),
            FieldMap.Compute(LookupSourceAttribute, month, s => s.Substring(4, 2)),
            FieldMap.Compute(LookupSourceAttribute, day, s => s.Substring(6, 2)),
            FieldMap.Compute(LookupSourceAttribute, hour, s => s.Substring(8, 2)),
            FieldMap.Compute(LookupSourceAttribute, duration, s => "1")
        };

        public TimeDimension()
        {
            Name = "time_dimension";
            LookupSourceAttribute = new SourceField("AAAAMMJJHH", SourceFieldType.String);

            yyyyMMddHH = Field.TextAttribute(Name, "yyyyMMddHH");
            timestamp = Field.IntAttribute(Name, "timestamp");
            year = Field.IntAttribute(Name, "year");
            month = Field.IntAttribute(Name, "month");
            day = Field.IntAttribute(Name, "day");
            hour = Field.IntAttribute(Name, "hour");
            duration = Field.IntAttribute(Name, "duration");
        }

        public static DateTime ToDateTime(string yyyyMMddHH) => DateTime.SpecifyKind(DateTime.ParseExact(yyyyMMddHH, "yyyyMMddHH", CultureInfo.InvariantCulture),
            DateTimeKind.Utc
        );

        public static string ToTimestamp(string yyyyMMddHH) => new DateTimeOffset(ToDateTime(yyyyMMddHH)).ToUnixTimeSeconds().ToString();

        public static DateTime FromTimestamp(string timestamp) => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timestamp)).UtcDateTime;
    }
}
