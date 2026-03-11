namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;

    public class WeatherFact : IFact
    {
        public string TableName => "hourly_weather_history_facts";
        public Field PrimaryKey => Field.PK();

        public static Field temperature = Field.Metric("temperature");
        public static Field temperature_10 = Field.Metric("temperature_10");
        public static Field temperature_20 = Field.Metric("temperature_20");
        public static Field temperature_100 = Field.Metric("temperature_100");
        public static Field rain_1 = Field.Metric("rain_1");
        public static Field wind_1 = Field.Metric("wind_1");
        public static Field wind_2 = Field.Metric("wind_2");
        public static Field gust_1 = Field.Metric("gust_1");
        public static Field gust_2 = Field.Metric("gust_2");
        public static Field gust_3s = Field.Metric("gust_3s");

        public Field timeRef, locationRef;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, timeRef, locationRef)
        };

        public List<IDimension> Dimensions => _dimensions;
        public List<Field> References => new List<Field>() { timeRef, locationRef };

        public List<FieldMapping> Mappings => new List<FieldMapping>()
        {
            FieldMapping.Map("T", temperature),
            FieldMapping.Map("RR1", rain_1)
        };

        private List<IDimension> _dimensions;

        public WeatherFact(TimeDimension time, LocationDimension location)
        {
            _dimensions = new List<IDimension>() { time, location };
            timeRef = Field.Ref(time, "time_id", "AAAAMMJJHH");
            locationRef = Field.Ref(location, "location_id", "NUM_POSTE");
        }
    }
}
