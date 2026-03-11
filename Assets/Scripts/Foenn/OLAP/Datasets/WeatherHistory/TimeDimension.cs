namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class TimeDimension : IDimension
    {
        public static Field yyyymmddhh = Field.Text("yyyymmddhh");
        public static Field year = Field.Int16("year");
        public static Field month = Field.Int16("month");
        public static Field day = Field.Int16("day");
        public static Field hour = Field.Int16("hour");

        public string TableName => "time";
        public Field PrimaryKey => Field.PK();

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, yyyymmddhh)
        };

        public List<FieldMapping> Mappings => new List<FieldMapping>()
        {
            FieldMapping.Map("AAAAMMJJHH", yyyymmddhh),
            FieldMapping.Compute("AAAAMMJJHH", Field.Int64("timestamp"), ToTimestamp),
            FieldMapping.Compute("AAAAMMJJHH", year, s => ParseDate(s).Year),
            FieldMapping.Compute("AAAAMMJJHH", month, s => ParseDate(s).Month),
            FieldMapping.Compute("AAAAMMJJHH", day, s => ParseDate(s).Day),
            FieldMapping.Compute("AAAAMMJJHH", hour, s => ParseDate(s).Hour),
            FieldMapping.Compute("AAAAMMJJHH", Field.Int16("day_of_week"), s => (int)ParseDate(s).DayOfWeek),
            FieldMapping.Compute("AAAAMMJJHH", Field.Int16("day_of_year"), s => ParseDate(s).DayOfYear),
            FieldMapping.Compute("AAAAMMJJHH", Field.Int16("week_of_year"), s => GetWeekOfYear(ParseDate(s)))
        };

        private static DateTime ParseDate(string s) => DateTime.ParseExact(s, "yyyyMMddHH", CultureInfo.InvariantCulture);
        private static object ToTimestamp(string s) => new DateTimeOffset(ParseDate(s)).ToUnixTimeSeconds();
        private static int GetWeekOfYear(DateTime d) => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
    }
}
