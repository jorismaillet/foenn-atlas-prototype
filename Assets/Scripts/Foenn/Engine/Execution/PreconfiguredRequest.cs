using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Unity;
using System.Collections.Generic;
using System.Diagnostics;


namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class PreconfiguredRequest
    {
        public static List<GeoMeasure> WeatherHistoryMeasures(string AAAAMMJJHH, string dpt, WeatherHistoryMetricKey metricKey)
        {
            var query = new QueryRequest(WeatherHistoryDatasource.tableName)
                .Select(new Attribute(WeatherHistoryAttributeKey.LON))
                .Select(new Attribute(WeatherHistoryAttributeKey.LAT))
                .Select(new Attribute(WeatherHistoryAttributeKey.NOM_USUEL))
                .Select(new Metric(metricKey, AggregationKey.AVG))
                .GroupBy(WeatherHistoryAttributeKey.NOM_USUEL)
                .Where(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.DPT, dpt))
                .Where(new ExcludeNullFilter(metricKey))
                .Where(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.AAAAMMJJHH, AAAAMMJJHH));
            var connector = new SqliteConnector();
            var result = connector.ExecuteQuery(query);
            var res = new List<GeoMeasure>();
            foreach (var row in result.rows)
            {
                var station = row.attributes[WeatherHistoryAttributeKey.NOM_USUEL].value;
                var point = row.geo.point;
                var measure = row.measures[metricKey];
                res.Add(new GeoMeasure(new PointLocation(station, point.lat, point.lon), measure));
            }
            return res;
        }
    }
}
