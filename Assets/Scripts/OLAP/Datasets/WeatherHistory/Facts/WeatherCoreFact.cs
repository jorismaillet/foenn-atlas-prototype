using System.Linq;
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

            Indexes.Add(new IndexDefinition(false, timeRef, rain, locationRef)); // Optimisation for rain case

            Mappings.Add(new FieldMap(temperature,
                new SourceField("T", SourceFieldType.Float),
                new SourceField("T10", SourceFieldType.Float),
                new SourceField("T20", SourceFieldType.Float),
                new SourceField("T50", SourceFieldType.Float),
                new SourceField("T100", SourceFieldType.Float)
            ));

            Mappings.Add(new FieldMap(new SourceField("TD", SourceFieldType.Float), dewPoint));
            Mappings.Add(new FieldMap(new SourceField("U", SourceFieldType.Float), humidity));
            Mappings.Add(new FieldMap(new SourceField("RR1", SourceFieldType.Float), rain));
            Mappings.Add(new FieldMap(new SourceField("FF", SourceFieldType.Float), windSpeed));
            Mappings.Add(new FieldMap(new SourceField("DD", SourceFieldType.Float), windDirection));
        }
    }
}
