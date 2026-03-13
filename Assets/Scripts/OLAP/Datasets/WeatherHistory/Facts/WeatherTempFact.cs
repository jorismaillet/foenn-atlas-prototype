using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.coreFacts
{
    public class WeatherTempFact : IFact
    {
        public string name => Name;
        public static string Name => "weather_vertical_temp_facts";

        public Field PrimaryKey => Field.PK(Name);

        public static Field temperature10 = Field.FloatMetric(Name, "temperature_10");
        public static Field temperature20 = Field.FloatMetric(Name, "temperature_20");
        public static Field temperature50 = Field.FloatMetric(Name, "temperature_50");
        public static Field temperature100 = Field.FloatMetric(Name, "temperature_100");

        public Field timeRef, locationRef;

        public List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, timeRef, locationRef)
        };

        public List<IDimension> Dimensions => _dimensions;

        public List<Field> References => new List<Field>() { timeRef, locationRef };

        public List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(new SourceAttribute("T10", SourceAttributeType.Float), temperature10),
            FieldMap.Map(new SourceAttribute("T20", SourceAttributeType.Float), temperature20),
            FieldMap.Map(new SourceAttribute("T50", SourceAttributeType.Float), temperature50),
            FieldMap.Map(new SourceAttribute("T100", SourceAttributeType.Float), temperature100),
        };

        private List<IDimension> _dimensions;

        public WeatherTempFact(TimeDimension time, LocationDimension location)
        {
            _dimensions = new List<IDimension>() { time, location };
            timeRef = Field.Ref(this, time, "time_id");
            locationRef = Field.Ref(this, location, "location_id");
        }
    }
}
