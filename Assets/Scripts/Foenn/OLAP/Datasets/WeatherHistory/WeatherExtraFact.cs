namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;
    using System.Data;

    public class WeatherExtraFact : IFact
    {
        public string Name => "hourly_weather_history_extra_facts";
        public PrimaryKey PrimaryKey => new PrimaryKey("ID", DbType.Int32, ColumnType.ATTRIBUTE, true);

        public static Field temperature_10 = new Field("temperature_10", DbType.Double, ColumnType.METRIC);
        public static Field temperature_20 = new Field("temperature_20", DbType.Double, ColumnType.METRIC);
        public static Field temperature_100 = new Field("temperature_100", DbType.Double, ColumnType.METRIC);
        public static Field wind_2 = new Field("wind_2", DbType.Double, ColumnType.METRIC);
        public static Field gust_2 = new Field("gust_2", DbType.Double, ColumnType.METRIC);
        public static Field gust_3s = new Field("gust_3s", DbType.Double, ColumnType.METRIC);

        public static Field timeId = new Field("time_id", DbType.Int32, ColumnType.ATTRIBUTE);
        public static Field locationId = new Field("location_id", DbType.Int32, ColumnType.ATTRIBUTE);

        public Reference timeReference, locationReference;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, timeId, locationId)
        };

        public List<Field> Columns => new List<Field> { PrimaryKey, timeId, locationId, temperature_10, temperature_20, temperature_100, wind_2, gust_2, gust_3s };
        public List<IDimension> Dimensions => _dimensions;
        public List<Reference> References => new List<Reference>() { timeReference, locationReference };

        private List<IDimension> _dimensions;

        public WeatherExtraFact(TimeDimension time, LocationDimension location)
        {
            _dimensions = new List<IDimension>() { time, location };
            timeReference = new Reference(time, timeId, "AAAAMMJJHH");
            locationReference = new Reference(location, locationId, "NUM_POSTE");
        }
    }
}
