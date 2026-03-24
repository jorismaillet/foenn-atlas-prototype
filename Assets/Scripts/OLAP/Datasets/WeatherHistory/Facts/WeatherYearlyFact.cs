using System.Collections.Generic;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;
using Mono.Data.Sqlite;

namespace Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts
{
    public class WeatherYearlyFact : Fact
    {
        public readonly Field
            year,
            locationRef,
            temperatureMin, temperatureAvg, temperatureMax,
            dewPointMin, dewPointAvg, dewPointMax,
            humidityMin, humidityAvg, humidityMax,
            rainMin, rainAvg, rainMax,
            windSpeedMin, windSpeedAvg, windSpeedMax;

        public WeatherYearlyFact(WeatherCoreFact coreFact, LocationDimension location) : base("weather_yearly_facts", new List<Dimension>() { location })
        {
            year  = Field.IntAttribute(Name, "year",  "Year");
            locationRef = Field.Ref(this, location, "location_id");

            temperatureMin = Field.FloatMetric(Name, "temperature_min", "Temperature Min", "°C");
            temperatureAvg = Field.FloatMetric(Name, "temperature_avg", "Temperature Avg", "°C");
            temperatureMax = Field.FloatMetric(Name, "temperature_max", "Temperature Max", "°C");

            dewPointMin = Field.FloatMetric(Name, "dew_point_min", "Dew Point Min", "°C");
            dewPointAvg = Field.FloatMetric(Name, "dew_point_avg", "Dew Point Avg", "°C");
            dewPointMax = Field.FloatMetric(Name, "dew_point_max", "Dew Point Max", "°C");

            humidityMin = Field.FloatMetric(Name, "humidity_min", "Humidity Min", "%");
            humidityAvg = Field.FloatMetric(Name, "humidity_avg", "Humidity Avg", "%");
            humidityMax = Field.FloatMetric(Name, "humidity_max", "Humidity Max", "%");

            rainMin = Field.FloatMetric(Name, "rain_min", "Rain Min", "mm");
            rainAvg = Field.FloatMetric(Name, "rain_avg", "Rain Avg", "mm");
            rainMax = Field.FloatMetric(Name, "rain_max", "Rain Max", "mm");

            windSpeedMin = Field.FloatMetric(Name, "wind_speed_min", "Wind Speed Min", "ms");
            windSpeedAvg = Field.FloatMetric(Name, "wind_speed_avg", "Wind Speed Avg", "ms");
            windSpeedMax = Field.FloatMetric(Name, "wind_speed_max", "Wind Speed Max", "ms");

            References.Add(locationRef);

            Indexes.Add(new IndexDefinition(true, year, locationRef));
            Indexes.Add(new IndexDefinition(false, locationRef));

            aggregatedField = new Dictionary<(Field, FieldAggregation), Field>
            {
                { (coreFact.temperature, FieldAggregation.Min), temperatureMin },
                { (coreFact.temperature, FieldAggregation.Avg), temperatureAvg },
                { (coreFact.temperature, FieldAggregation.Max), temperatureMax },
                { (coreFact.dewPoint,    FieldAggregation.Min), dewPointMin },
                { (coreFact.dewPoint,    FieldAggregation.Avg), dewPointAvg },
                { (coreFact.dewPoint,    FieldAggregation.Max), dewPointMax },
                { (coreFact.humidity,    FieldAggregation.Min), humidityMin },
                { (coreFact.humidity,    FieldAggregation.Avg), humidityAvg },
                { (coreFact.humidity,    FieldAggregation.Max), humidityMax },
                { (coreFact.rain,        FieldAggregation.Min), rainMin },
                { (coreFact.rain,        FieldAggregation.Avg), rainAvg },
                { (coreFact.rain,        FieldAggregation.Max), rainMax },
                { (coreFact.windSpeed,   FieldAggregation.Min), windSpeedMin },
                { (coreFact.windSpeed,   FieldAggregation.Avg), windSpeedAvg },
                { (coreFact.windSpeed,   FieldAggregation.Max), windSpeedMax }
            };
        }

        public readonly Dictionary<(Field fieldName, FieldAggregation aggregation), Field> aggregatedField;

        public override void BuildDerivedFact(SqliteConnection connection)
        {
            var coreFact = WeatherHistoryDataset.Instance.coreFact;

            var sql = $@"
                INSERT OR IGNORE INTO {Name} (year, location_id,
                    temperature_min, temperature_avg, temperature_max,
                    dew_point_min, dew_point_avg, dew_point_max,
                    humidity_min, humidity_avg, humidity_max,
                    rain_min, rain_avg, rain_max,
                    wind_speed_min, wind_speed_avg, wind_speed_max)
                SELECT t.year, cf.location_id,
                    MIN(cf.temperature), AVG(cf.temperature), MAX(cf.temperature),
                    MIN(cf.dew_point),   AVG(cf.dew_point),   MAX(cf.dew_point),
                    MIN(cf.humidity),    AVG(cf.humidity),    MAX(cf.humidity),
                    MIN(cf.rain),        AVG(cf.rain),        MAX(cf.rain),
                    MIN(cf.wind_speed),  AVG(cf.wind_speed),  MAX(cf.wind_speed)
                FROM {coreFact.Name} cf
                JOIN {coreFact.dimensions[0].Name} t ON cf.time_id = t.id
                GROUP BY t.year, cf.location_id";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public override IEnumerable<Field> Columns
        {
            get
            {
                yield return PrimaryKey;
                yield return year;
                yield return temperatureMin; yield return temperatureAvg; yield return temperatureMax;
                yield return dewPointMin;    yield return dewPointAvg;    yield return dewPointMax;
                yield return humidityMin;    yield return humidityAvg;    yield return humidityMax;
                yield return rainMin;        yield return rainAvg;        yield return rainMax;
                yield return windSpeedMin;   yield return windSpeedAvg;   yield return windSpeedMax;
                yield return locationRef;
            }
        }
    }
}
