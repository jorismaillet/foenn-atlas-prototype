namespace Assets.Scripts.Foenn.OLAP.Query
{
    using Assets.Scripts.Foenn.Atlas.Models.Geo;
    using Assets.Scripts.Foenn.Atlas.Models.Locations;
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory;
    using Assets.Scripts.Foenn.OLAP.Fields;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Assets.Scripts.Foenn.OLAP.Sql;
    using System.Collections.Generic;

    public class PreconfiguredRequest
    {
        public static List<GeoMeasure> FieldMeasuresPerPostForDay(int dayOfMonth, int month, int year, string dpt, WeatherHistoryMetricKey key)
        {
            var factField = FieldFor(key);
            var postName = new PrefixedField(WeatherHistoryDataset.location, LocationDimension.PostName);
            var avgFact = new PrefixedAggregatedField(WeatherHistoryDataset.fact, factField, AggregationKey.AVG);
            var query = new QueryRequest(WeatherHistoryDataset.fact)
                .Select(
                    LocationDimension.Longitude,
                    LocationDimension.Latitude,
                    LocationDimension.PostName,
                    avgFact)
                .Join(
                    WeatherHistoryDataset.fact,
                    WeatherHistoryDataset.fact.locationReference,
                    JoinType.INNER)
                .Join(
                    WeatherHistoryDataset.fact,
                    WeatherHistoryDataset.fact.timeReference,
                    JoinType.INNER)
                .GroupBy(
                    LocationDimension.PostName)
                .Where(
                    new DataFilter(new PrefixedField(
                        WeatherHistoryDataset.location,
                        LocationDimension.Department),
                        DataFilterMode.INCLUDE,
                        dpt))
                .Where(
                    new DataFilter(new PrefixedField(
                        WeatherHistoryDataset.time,
                        TimeDimension.day),
                        DataFilterMode.INCLUDE,
                        dayOfMonth))
                .Where(
                    new DataFilter(new PrefixedField(
                        WeatherHistoryDataset.time,
                        TimeDimension.month),
                        DataFilterMode.INCLUDE,
                        month))
                .Where(
                    new DataFilter(new PrefixedField(
                        WeatherHistoryDataset.time,
                        TimeDimension.year),
                        DataFilterMode.INCLUDE,
                        year))
                .Where(
                    new ExcludeNullFilter(new PrefixedField(
                        WeatherHistoryDataset.fact,
                        WeatherFact.temperature)));
            using (var connection = SqliteHelper.CreateConnection())
            {
                var result = query.Execute(connection);
                var res = new List<GeoMeasure>();
                foreach (var row in result.rows)
                {
                    string post = (string)row.values[LocationDimension.PostName];
                    var point = row.geo.point;
                    var measure = new Measure(factField, (float)row.values[avgFact]);
                    res.Add(new GeoMeasure(new PointLocation(post, point.lat, point.lon), measure));
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
