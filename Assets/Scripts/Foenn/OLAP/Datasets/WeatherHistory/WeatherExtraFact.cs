namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;

    public class WeatherExtraFact : IFact
    {
        public string TableName => "hourly_weather_history_extra_facts";
        public Field PrimaryKey => Field.PK();

        public Field timeRef, locationRef;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, timeRef, locationRef)
        };

        public List<IDimension> Dimensions => _dimensions;
        public List<Field> References => new List<Field>() { timeRef, locationRef };

        public List<FieldMapping> Mappings => new List<FieldMapping>()
        {
            FieldMapping.Map("T10", "temperature_10"),
            FieldMapping.Map("T20", "temperature_20"),
            FieldMapping.Map("T100", "temperature_100"),
            FieldMapping.Map("FF2", "wind_2"),
            FieldMapping.Map("FXI2", "gust_2"),
            FieldMapping.Map("FXI3S", "gust_3s")
        };

        private List<IDimension> _dimensions;

        public WeatherExtraFact(TimeDimension time, LocationDimension location)
        {
            _dimensions = new List<IDimension>() { time, location };
            timeRef = Field.Ref(time, "time_id", "AAAAMMJJHH");
            locationRef = Field.Ref(location, "location_id", "NUM_POSTE");
        }
    }
}
