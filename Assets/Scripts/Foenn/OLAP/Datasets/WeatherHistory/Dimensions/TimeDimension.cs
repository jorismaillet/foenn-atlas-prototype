namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class TimeDimension : IDimension
    {
        public string TableName => "time";
        public Field PrimaryKey => Field.PK();

        public static Field timestamp = Field.Int64("timestamp");
        public static Field year = Field.Int16("year");
        public static Field month = Field.Int16("month");
        public static Field day = Field.Int16("day");
        public static Field hour = Field.Int16("hour");

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
        };

        private static SourceAttribute AAAAMMJJHH = new SourceAttribute("AAAAMMJJHH", SourceAttributeType.String);

        public List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Compute(AAAAMMJJHH, timestamp, ToTimestamp),
            FieldMap.Compute(AAAAMMJJHH, year, s => s.Substring(0, 4)),
            FieldMap.Compute(AAAAMMJJHH, month, s => s.Substring(4, 2)),
            FieldMap.Compute(AAAAMMJJHH, day, s => s.Substring(6, 2)),
            FieldMap.Compute(AAAAMMJJHH, hour, s => s.Substring(8, 2))
        };

        private static DateTime ParseDate(string s) => DateTime.ParseExact(s, "yyyyMMddHH", CultureInfo.InvariantCulture);
        private static string ToTimestamp(string s) => new DateTimeOffset(ParseDate(s)).ToUnixTimeSeconds().ToString();
    }
}
