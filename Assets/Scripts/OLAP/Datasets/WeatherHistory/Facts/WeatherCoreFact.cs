using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts
{
    public class WeatherCoreFact : WeatherBaseFact
    {
        public readonly Field
            temperature,
            dewPoint,
            humidity,
            rain,
            windSpeed,
            windDirection;

        public WeatherCoreFact(TimeDimension time, LocationDimension location) : base("weather_history_facts", time, location)
        {
            temperature = Field.FloatMetric(Name, "temperature", "Temperature", "°C");
            dewPoint = Field.FloatMetric(Name, "dew_point", "Dew point", "°C");
            humidity = Field.FloatMetric(Name, "humidity", "Humidity", "%");
            rain = Field.FloatMetric(Name, "rain", "Rain", "mm");
            windSpeed = Field.FloatMetric(Name, "wind_speed", "Wind speed", "ms");
            windDirection = Field.FloatMetric(Name, "wind_direction", "Wind direction", "°");

            Mappings.Add(FieldMap.Map(new SourceField("T", SourceFieldType.Float), temperature));
            Mappings.Add(FieldMap.Map(new SourceField("TD", SourceFieldType.Float), dewPoint));
            Mappings.Add(FieldMap.Map(new SourceField("U", SourceFieldType.Float), humidity));
            Mappings.Add(FieldMap.Map(new SourceField("RR1", SourceFieldType.Float), rain));
            Mappings.Add(FieldMap.Map(new SourceField("FF", SourceFieldType.Float), windSpeed));
            Mappings.Add(FieldMap.Map(new SourceField("DD", SourceFieldType.Float), windDirection));
        }
    }
}
