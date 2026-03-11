namespace Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Collections.Generic;

    public class WeatherExtraFact : IFact
    {
        public string TableName => "weather_history_extra_facts";
        public Field PrimaryKey => Field.PK();

        public static Field temperature10 = Field.Metric("temperature_10");
        public static Field temperature20 = Field.Metric("temperature_20");

        public Field timeRef, locationRef;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, timeRef, locationRef)
        };

        public List<IDimension> Dimensions => _dimensions;
        public List<Field> References => new List<Field>() { timeRef, locationRef };

        public List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(new SourceAttribute("T10", SourceAttributeType.Float), temperature10),
            FieldMap.Map(new SourceAttribute("T20", SourceAttributeType.Float), temperature20)
        };

        private List<IDimension> _dimensions;

        public WeatherExtraFact(TimeDimension time, LocationDimension location)
        {
            _dimensions = new List<IDimension>() { time, location };
            timeRef = Field.Ref(time, "time_id");
            locationRef = Field.Ref(location, "location_id");
        }
    }
}
