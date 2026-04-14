using System.Collections.Generic;
using Assets.Scripts.ETL.Loaders;
using Assets.Scripts.Helpers;
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

        private WeatherCoreFact derivedFromFact;

        public WeatherYearlyFact(WeatherCoreFact coreFact, LocationDimension location) : base("weather_yearly_facts", new List<Dimension>() { location })
        {
            this.derivedFromFact = coreFact;

            year = Field.IntAttribute(Name, "year", "Year");
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

        public override void BuildDerivedFact(SqliteConnection connection, Dictionary<Dimension, DimensionCache> caches)
        {
            var insertColumns = new List<string>();
            var selectExpressions = new List<string>();

            foreach (var entry in aggregatedField)
            {
                var (sourceField, aggregation) = entry.Key;
                insertColumns.Add(entry.Value.fieldName);
                selectExpressions.Add($"{aggregation.ToString().ToUpper()}(cf.{sourceField.fieldName})");
            }

            SqliteHelper.ExecuteRaw(connection, SqliteHelper.InsertFromTableSQL(
                caches[dimensions[0]].AccessedIds,
                this, derivedFromFact, derivedFromFact.time, derivedFromFact.timeRef,
                year, locationRef,
                insertColumns, selectExpressions));
        }

        public override IEnumerable<Field> Columns
        {
            get
            {
                yield return PrimaryKey;
                yield return year;
                yield return temperatureMin; yield return temperatureAvg; yield return temperatureMax;
                yield return dewPointMin; yield return dewPointAvg; yield return dewPointMax;
                yield return humidityMin; yield return humidityAvg; yield return humidityMax;
                yield return rainMin; yield return rainAvg; yield return rainMax;
                yield return windSpeedMin; yield return windSpeedAvg; yield return windSpeedMax;
                yield return locationRef;
            }
        }
    }
}
