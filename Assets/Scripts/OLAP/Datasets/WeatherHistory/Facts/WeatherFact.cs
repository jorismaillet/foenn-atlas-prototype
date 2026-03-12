using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts
{
    public class WeatherFact : IFact
    {
        public string name => "weather_history_facts";

        public Field PrimaryKey => Field.PK();

        public static Field temperature = Field.FloatMetric("temperature");
        public static Field rain = Field.FloatMetric("rain");
        public static Field wind = Field.FloatMetric("wind");
        public static Field gust = Field.FloatMetric("gust");

        public Field timeRef, locationRef;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, timeRef, locationRef)
        };

        public List<IDimension> Dimensions => _dimensions;

        public List<Field> References => new List<Field>() { timeRef, locationRef };

        public List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(new SourceAttribute("T", SourceAttributeType.Float), temperature),
            FieldMap.Map(new SourceAttribute("RR1", SourceAttributeType.Float), rain),
            FieldMap.Map(new SourceAttribute("FF", SourceAttributeType.Float), wind),
            FieldMap.Map(new SourceAttribute("FXI", SourceAttributeType.Float), gust),
        };

        private List<IDimension> _dimensions;

        public WeatherFact(TimeDimension time, LocationDimension location)
        {
            _dimensions = new List<IDimension>() { time, location };
            timeRef = Field.Ref(this, time, "time_id");
            locationRef = Field.Ref(this, location, "location_id");
        }
    }
}
