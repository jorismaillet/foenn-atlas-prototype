using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.Models.Locations;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.Dimensions;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.coreFacts;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.Services
{
    public class WeatherQueryService
    {
        public static List<GeoMeasure> DayObservationsForPost(int dayOfMonth, int month, int year, string dpt, string key)
        {
            var factTable = WeatherHistoryDataset.coreFact;
            var fieldToMeasure = FieldFor(key);
            var query = new QueryRequest(factTable)
                .Select(
                    LocationDimension.Longitude,
                    LocationDimension.Latitude,
                    LocationDimension.PostName)
                .SelectAvg(fieldToMeasure)
                .Join(factTable.locationRef)
                .Join(factTable.timeRef)
                .GroupBy(LocationDimension.PostName)
                .WhereEq(LocationDimension.Department, dpt)
                .WhereEq(TimeDimension.day, dayOfMonth)
                .WhereEq(TimeDimension.month, month)
                .WhereEq(TimeDimension.year, year)
                .WhereNotNull(WeatherCoreFact.temperature);

            using (var connection = SqliteHelper.CreateConnection())
            {
                var result = query.Execute(connection);
                var res = new List<GeoMeasure>();
                foreach (var row in result.rows)
                {
                    string postName = (string)row.values[LocationDimension.PostName];
                    var measure = row.FloatValue(fieldToMeasure);
                    res.Add(new GeoMeasure(new PointLocation(postName, row.geo.lat, row.geo.lon), fieldToMeasure, measure));
                }
                return res;
            }
        }

        private static Field FieldFor(string key)
        {
            return key switch
            {
                "temperature" => WeatherCoreFact.temperature,
                "rain" => WeatherCoreFact.rain,
                _ => throw new KeyNotFoundException($"No field found for metric {key}")
            };
        }
    }
}
