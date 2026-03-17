using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts
{
    public class WeatherWindFact : WeatherBaseFact
    {
        public readonly Field
            gust,
            gust3S;

        public WeatherWindFact(TimeDimension time, LocationDimension location) : base("weather_advanced_wind_facts", time, location)
        {
            gust = Field.FloatMetric(Name, "wind_gust", "Gust", "kmh");
            gust3S = Field.FloatMetric(Name, "wind_gust_3s", "Gust (Max 3s)", "kmh");

            Mappings.Add(FieldMap.Map(new SourceField("FXI", SourceFieldType.Float), gust));
            Mappings.Add(FieldMap.Map(new SourceField("FXI3S", SourceFieldType.Float), gust3S));
        }
    }
}
