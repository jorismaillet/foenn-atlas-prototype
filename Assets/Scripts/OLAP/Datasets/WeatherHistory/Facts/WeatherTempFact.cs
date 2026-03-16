using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts
{
    public class WeatherTempFact : Fact
    {
        public override string Name { get; }

        public Field
            temperature10,
            temperature20,
            temperature50,
            temperature100,
            timeRef,
            locationRef;

        public override List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, timeRef, locationRef)
        };

        public override List<Field> References => new List<Field>() { timeRef, locationRef };

        public override List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(new SourceField("T10", SourceFieldType.Float), temperature10),
            FieldMap.Map(new SourceField("T20", SourceFieldType.Float), temperature20),
            FieldMap.Map(new SourceField("T50", SourceFieldType.Float), temperature50),
            FieldMap.Map(new SourceField("T100", SourceFieldType.Float), temperature100),
        };

        public WeatherTempFact(TimeDimension time, LocationDimension location) : base(new List<Dimension>() { time, location })
        {
            Name = "weather_vertical_temp_facts";
            temperature10 = Field.FloatMetric(Name, "temperature_10");
            temperature20 = Field.FloatMetric(Name, "temperature_20");
            temperature50 = Field.FloatMetric(Name, "temperature_50");
            temperature100 = Field.FloatMetric(Name, "temperature_100");

            timeRef = Field.Ref(this, time, "time_id");
            locationRef = Field.Ref(this, location, "location_id");
        }
    }
}
