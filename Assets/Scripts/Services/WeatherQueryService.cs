using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.Models.Locations;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Facts;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Engine.Result;
using Assets.Scripts.OLAP.Engine.Sql.Clauses;
using Assets.Scripts.OLAP.Engine.Sql.Filters;
using Assets.Scripts.OLAP.Engine.Sql.Joins;
using Assets.Scripts.OLAP.Schema;
using SqlKata;

namespace Assets.Scripts.Services
{
    public class WeatherQueryService
    {
        public static List<GeoMeasure> DayObservationsForPost(int dayOfMonth, int month, int year, string dpt, string key)
        {
            var factTable = WeatherHistoryDataset.fact;
            var observation = FieldFor(key).Of(factTable);
            var query = QueryRequest.From(factTable)
                .Select(
                    LocationDimension.Longitude,
                    LocationDimension.Latitude,
                    LocationDimension.PostName)
                .SelectAvg(observation)
                .Join(factTable.locationRef)
                .Join(factTable.timeRef)
                .GroupBy(LocationDimension.PostName)
                .WhereEq(LocationDimension.Department, dpt)
                .WhereEq(TimeDimension.day, dayOfMonth)
                .WhereEq(TimeDimension.month, month)
                .WhereEq(TimeDimension.year, year)
                .WhereNotNull(WeatherFact.temperature);

            using (var connection = SqliteHelper.CreateConnection())
            {
                var result = query.Execute(connection);
                var res = new List<GeoMeasure>();
                foreach (var row in result.rows)
                {
                    string post = (string)row.values[LocationDimension.PostName];
                    var coord = row.geo;
                    var measure = new Measure(observation, (float)row.values[avgFact]);
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
