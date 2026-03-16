using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.coreFacts
{
    public class WeatherWindFact : Fact
    {
        public override string Name { get; }

        public Field
            gust,
            gust3S,
            timeRef,
            locationRef;

        public override List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, timeRef, locationRef)
        };

        public override List<Field> References => new List<Field>() { timeRef, locationRef };

        public override List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(new SourceField("FXI", SourceFieldType.Float), gust),
            FieldMap.Map(new SourceField("FXI3S", SourceFieldType.Float), gust3S),
        };

        public WeatherWindFact(TimeDimension time, LocationDimension location) : base(new List<Dimension>() { time, location })
        {
            Name = "weather_advanced_wind_facts";

            gust = Field.FloatMetric(Name, "wind_gust");
            gust3S = Field.FloatMetric(Name, "wind_gust_3s");

            timeRef = Field.Ref(this, time, "time_id");
            locationRef = Field.Ref(this, location, "location_id");
        }
    }
}
