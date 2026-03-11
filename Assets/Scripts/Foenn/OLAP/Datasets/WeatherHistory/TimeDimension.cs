namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;
    using System.Data;

    public class TimeDimension : IDimension
    {
        public static Field timestamp = new Field("timestamp", DbType.Int16, ColumnType.ATTRIBUTE);

        public static Field yyyymmddhh = new Field("yyyymmddhh", DbType.String, ColumnType.ATTRIBUTE);

        public static Field year = new Field("year", DbType.Int16, ColumnType.ATTRIBUTE);

        public static Field month = new Field("month", DbType.Int16, ColumnType.ATTRIBUTE);

        public static Field day = new Field("day", DbType.Int16, ColumnType.ATTRIBUTE);

        public static Field hour = new Field("hour", DbType.Int16, ColumnType.ATTRIBUTE);

        public static Field dayOfWeek = new Field("day_of_week", DbType.Int16, ColumnType.ATTRIBUTE);

        public static Field dayOfYear = new Field("day_of_year", DbType.Int16, ColumnType.ATTRIBUTE);

        public static Field weekOfYear = new Field("week_of_year", DbType.Int16, ColumnType.ATTRIBUTE);

        public string Name => "time";

        public PrimaryKey PrimaryKey => new PrimaryKey("ID", DbType.Int64, ColumnType.ATTRIBUTE, true);

        public List<IndexDefinition> Indexes => new List<IndexDefinition>()
        {
        };

        public List<Field> Columns => new List<Field>() { timestamp, yyyymmddhh, year, month, day, hour, dayOfWeek, dayOfYear, weekOfYear };

        public List<Reference> References => new List<Reference>();
    }
}
