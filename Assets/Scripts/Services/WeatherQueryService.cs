namespace Assets.Scripts.Foenn.Atlas.Services
{
    using Assets.Scripts.Foenn.Atlas.Models.Geo;
    using Assets.Scripts.Foenn.Atlas.Models.Locations;
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory;
    using Assets.Scripts.Foenn.OLAP.Query;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Assets.Scripts.Foenn.OLAP.Sql;
    using System.Collections.Generic;

    public class WeatherQueryService
    {
        public static List<GeoMeasure> FieldMeasuresPerPostForDay(int dayOfMonth, int month, int year, string dpt, WeatherHistoryMetricKey key)
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
                                var coord = row.geo.point;
                                var measure = new Measure(factField, (float)row.values[avgFact]);
                                res.Add(new GeoMeasure(new PointLocation(post, coord.lat, coord.lon), measure));
                            }
                            return res;
                        }
        }

        private static Field FieldFor(WeatherHistoryMetricKey key)
        {
            return key switch
            {
                WeatherHistoryMetricKey.T => WeatherFact.temperature,
                WeatherHistoryMetricKey.R => WeatherFact.rain_1,
                _ => throw new KeyNotFoundException($"No field found for metric {key}")
            };
        }
    }
}
