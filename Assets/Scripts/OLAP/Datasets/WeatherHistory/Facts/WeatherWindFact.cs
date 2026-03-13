using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.coreFacts
{
    public class WeatherWindFact : IFact
    {
        public string name => "weather_advanced_wind_facts";

        public Field PrimaryKey => Field.PK();

        public static Field gust = Field.FloatMetric("wind_gust");
        public static Field gust3S = Field.FloatMetric("wind_gust_3s");

        public Field timeRef, locationRef;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, timeRef, locationRef)
        };

        public List<IDimension> Dimensions => _dimensions;

        public List<Field> References => new List<Field>() { timeRef, locationRef };

        public List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(new SourceAttribute("FXI", SourceAttributeType.Float), gust),
            FieldMap.Map(new SourceAttribute("FXI3S", SourceAttributeType.Float), gust3S),
        };

        private List<IDimension> _dimensions;

        public WeatherWindFact(TimeDimension time, LocationDimension location)
        {
            _dimensions = new List<IDimension>() { time, location };
            timeRef = Field.Ref(this, time, "time_id");
            locationRef = Field.Ref(this, location, "location_id");
        }
    }
}
