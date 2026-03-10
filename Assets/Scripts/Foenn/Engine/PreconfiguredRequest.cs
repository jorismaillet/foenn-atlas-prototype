using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Datasets.Facts;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.Engine.Sql.Clauses;
using Assets.Scripts.Foenn.ETL.Datasets;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Dimensions;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Unity;
using System.Collections.Generic;
using System.Diagnostics;


namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class PreconfiguredRequest
    {
        public static List<GeoMeasure> FieldMeasuresPerPostForDay(int dayOfMonth, int month, int year, string dpt, WeatherHistoryMetricKey key)
        {
            var factField = FieldFor(key);
            var postName = new PrefixedField(WeatherHistoryDataset.location, LocationDimension.PostName);
            var avgFact = new PrefixedField(WeatherHistoryDataset.fact, factField, AggregationKey.AVG);
            var minFact = new PrefixedField(WeatherHistoryDataset.fact, factField, AggregationKey.MIN);
            var maxFact = new PrefixedField(WeatherHistoryDataset.fact, factField, AggregationKey.MAX);
            var query = new QueryRequest(WeatherHistoryDataset.fact)
                .Select(
                    WeatherHistoryDataset.location,
                    LocationDimension.Longitude,
                    LocationDimension.Latitude,
                    LocationDimension.PostName)
                .Select(postNumber, avgFact, minFact, maxFact)
                .Join(
                    WeatherHistoryDataset.fact,
                    WeatherHistoryDataset.fact.locationReference,
                    JoinType.INNER)
                .Join(
                    WeatherHistoryDataset.fact,
                    WeatherHistoryDataset.fact.timeReference,
                    JoinType.INNER)
                .GroupBy(
                    WeatherHistoryDataset.location,
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
            using (var connection = SqliteHelper.CreateConnection()) {
                var result = query.Execute(connection);
                var res = new List<GeoMeasure>();
                foreach (var row in result.rows) {
                    string station = (string)row.values[LocationDimension.PostNumber];
                    var point = row.geo.point;
                    var value = (float)row.values[factField];
                    var measures = new List<Measure>() {
                        new Measure(avgFact, value),
                        new Measure(minFact, (float)row.values[minFact]),
                        new Measure(maxFact, (float)row.values[maxFact])
                    }

                    res.Add(new GeoMeasure(new PointLocation(station, point.lat, point.lon), new Measure(factField, value));
                }
                return res;
            }
        }

        private static Field FieldFor(WeatherHistoryMetricKey key) {
            return key switch
            {
                WeatherHistoryMetricKey.T => WeatherFact.temperature,
                WeatherHistoryMetricKey.R => WeatherFact.rain,
                _ => throw new KeyNotFoundException($"No field found for metric {key}")
            };

        }
    }
}
