using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.coreFacts
{
    public class WeatherCoreFact : Fact
    {
        public override string Name { get; }

        public Field
            temperature,
            dewPoint,
            humidity,
            rain,
            windSpeed,
            windDirection,
            timeRef,
            locationRef;

        public override List<IndexDefinition> Indexes => new List<IndexDefinition>() {
            new IndexDefinition(true, timeRef, locationRef)
        };

        public override List<Field> References => new List<Field>() { timeRef, locationRef };

        public override List<FieldMap> Mappings => new List<FieldMap>()
        {
            FieldMap.Map(new SourceField("T", SourceFieldType.Float), temperature),
            FieldMap.Map(new SourceField("TD", SourceFieldType.Float), dewPoint),
            FieldMap.Map(new SourceField("U", SourceFieldType.Float), humidity),
            FieldMap.Map(new SourceField("RR1", SourceFieldType.Float), rain),
            FieldMap.Map(new SourceField("FF", SourceFieldType.Float), windSpeed),
            FieldMap.Map(new SourceField("DD", SourceFieldType.Float), windDirection),
        };

        public WeatherCoreFact(TimeDimension time, LocationDimension location) : base(new List<Dimension>() { time, location })
        {
            Name = "weather_history_facts";
            temperature = Field.FloatMetric(Name, "temperature");
            dewPoint = Field.FloatMetric(Name, "dew_point");
            humidity = Field.FloatMetric(Name, "humidity");
            rain = Field.FloatMetric(Name, "rain");
            windSpeed = Field.FloatMetric(Name, "wind_speed");
            windDirection = Field.FloatMetric(Name, "wind_direction");

            timeRef = Field.Ref(this, time, "time_id");
            locationRef = Field.Ref(this, location, "location_id");
        }
    }
}
