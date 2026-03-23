using Assets.Scripts.Helpers;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions
{
    public class TimeDimension : Dimension
    {
        public override Field LookupField => yyyyMMddHH;

        public readonly Field
            yyyyMMddHH,
            timestamp,
            year,
            month,
            day,
            hour,
            duration;

        public TimeDimension() : base(
            "time_dimension",
            new SourceField("AAAAMMJJHH", SourceFieldType.String)
        )
        {
            yyyyMMddHH = Field.TextAttribute(Name, "yyyyMMddHH", "yyyyMMddHH");
            timestamp = Field.IntAttribute(Name, "timestamp", "Timestamp");
            year = Field.IntAttribute(Name, "year", "Year");
            month = Field.IntAttribute(Name, "month", "Month");
            day = Field.IntAttribute(Name, "day", "Day");
            hour = Field.IntAttribute(Name, "hour", "Hour");
            duration = Field.IntAttribute(Name, "duration", "Duration");

            Indexes.Add(new IndexDefinition(true, LookupField));
            Indexes.Add(new IndexDefinition(false, year, month, day));
            Indexes.Add(new IndexDefinition(false, year, month));
            Indexes.Add(new IndexDefinition(false, year, hour));
            Indexes.Add(new IndexDefinition(false, year));

            Mappings.Add(new FieldMap(LookupSourceAttribute, yyyyMMddHH));
            Mappings.Add(new FieldMap(LookupSourceAttribute, timestamp, s => DateTimeHelper.ToTimestamp(s)));
            Mappings.Add(new FieldMap(LookupSourceAttribute, year, s => s.Substring(0, 4)));
            Mappings.Add(new FieldMap(LookupSourceAttribute, month, s => s.Substring(4, 2)));
            Mappings.Add(new FieldMap(LookupSourceAttribute, day, s => s.Substring(6, 2)));
            Mappings.Add(new FieldMap(LookupSourceAttribute, hour, s => s.Substring(8, 2)));
            Mappings.Add(new FieldMap(LookupSourceAttribute, duration, s => "1"));
        }
     }
}
