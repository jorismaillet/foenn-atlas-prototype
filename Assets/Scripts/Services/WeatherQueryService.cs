using System.Collections.Generic;
using Assets.Scripts.Database;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.Models.Locations;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Engine;
using Assets.Scripts.OLAP.Schema.Fields;
using Assets.Scripts.OLAP.Schema.Tables;

namespace Assets.Scripts.Services
{
    public class WeatherQueryService
    {
        public static List<GeoMeasure> DayObservationsForPost(int dayOfMonth, int month, int year, string dpt, string coreFactkey)
        {
            var dataset = WeatherHistoryDataset.Instance;
            var coreFact = dataset.coreFact;
            var fieldToMeasure = FieldFor(dataset.coreFact, coreFactkey);
            var query = new QueryRequest(coreFact)
                .Select(
                    dataset.location.Longitude,
                    dataset.location.Latitude,
                    dataset.location.PostName)
                .SelectAvg(fieldToMeasure)
                .Join(coreFact.locationRef)
                .Join(coreFact.timeRef)
                .WhereEq(dataset.location.Department, dpt)
                .WhereEq(dataset.time.day, dayOfMonth)
                .WhereEq(dataset.time.month, month)
                .WhereEq(dataset.time.year, year)
                .WhereNotNull(fieldToMeasure);

            using (var connection = SqliteHelper.CreateConnection())
            {
                var result = query.Execute(connection);
                var res = new List<GeoMeasure>();
                foreach (var row in result.rows)
                {
                    string postName = (string)row.values[dataset.location.PostName];
                    var measure = row.FloatValue(fieldToMeasure);
                    res.Add(new GeoMeasure(new PointLocation(postName, row.geo.lat, row.geo.lon), fieldToMeasure, measure));
                }
                return res;
            }
        }

        private static Field FieldFor(Fact fact, string key)
        {
            return ((ITable)fact).Columns.Find(c => c.fieldName == key);
        }
    }
}
