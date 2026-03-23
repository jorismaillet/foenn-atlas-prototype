using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts
{
    public class WeatherTempFact : WeatherBaseFact
    {
        public readonly Field
            temperature10,
            temperature20,
            temperature50,
            temperature100;

        public WeatherTempFact(TimeDimension time, LocationDimension location) : base("weather_vertical_temp_facts", time, location)
        {
            temperature10 = Field.FloatMetric(Name, "temperature_10", "Temperature (10m)", "°C");
            temperature20 = Field.FloatMetric(Name, "temperature_20", "Temperature (20m)", "°C");
            temperature50 = Field.FloatMetric(Name, "temperature_50", "Temperature (50m)", "°C");
            temperature100 = Field.FloatMetric(Name, "temperature_100", "Temperature (100m)", "°C");

            Mappings.Add(new FieldMap(new SourceField("T10", SourceFieldType.Float), temperature10));
            Mappings.Add(new FieldMap(new SourceField("T20", SourceFieldType.Float), temperature20));
            Mappings.Add(new FieldMap(new SourceField("T50", SourceFieldType.Float), temperature50));
            Mappings.Add(new FieldMap(new SourceField("T100", SourceFieldType.Float), temperature100));
        }
    }
}
