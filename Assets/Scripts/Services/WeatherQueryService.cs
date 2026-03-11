using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.Models.Locations;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Engine.Result;
using Assets.Scripts.OLAP.Engine.Sql.Filters;
using Assets.Scripts.OLAP.Engine.Sql.Joins;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.Services
{
    public class WeatherQueryService
    {
        public static List<GeoMeasure> FieldMeasuresPerPostForDay(int dayOfMonth, int month, int year, string dpt, string key)
        {
            var factField = FieldFor(key);
            var loc = WeatherHistoryDataset.location;
            var time = WeatherHistoryDataset.time;
            var fact = WeatherHistoryDataset.fact;

            var avgFact = factField.Of(fact, AggregationKey.AVG);

            var query = new QueryRequest(fact)
                .Select(
                    LocationDimension.Longitude,
                    LocationDimension.Latitude,
                    LocationDimension.PostName,
                    avgFact)
                .Join(fact, fact.locationRef, JoinType.INNER)
                .Join(fact, fact.timeRef, JoinType.INNER)
                .GroupBy(LocationDimension.PostName)
                .Where(new DataFilter(LocationDimension.Department.Of(loc), DataFilterMode.INCLUDE, dpt))
                .Where(new DataFilter(TimeDimension.day.Of(time), DataFilterMode.INCLUDE, dayOfMonth))
                .Where(new DataFilter(TimeDimension.month.Of(time), DataFilterMode.INCLUDE, month))
                .Where(new DataFilter(TimeDimension.year.Of(time), DataFilterMode.INCLUDE, year))
                .Where(new ExcludeNullFilter(WeatherFact.temperature.Of(fact)));

            using (var connection = SqliteHelper.CreateConnection())
            {
                var result = query.Execute(connection);
                var res = new List<GeoMeasure>();
                foreach (var row in result.rows)
                {
                    string post = (string)row.values[LocationDimension.PostName];
                    var coord = row.geo;
                    var measure = new Measure(factField, (float)row.values[avgFact]);
                    res.Add(new GeoMeasure(new PointLocation(post, coord.lat, coord.lon), measure));
                }
                return res;
            }
        }

        private static Field FieldFor(string key)
        {
            return key switch
            {
                "temperature" => WeatherFact.temperature,
                "rain" => WeatherFact.rain,
                _ => throw new KeyNotFoundException($"No field found for metric {key}")
            };
        }
    }
}
