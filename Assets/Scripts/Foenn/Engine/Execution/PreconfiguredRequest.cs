using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;
using System.Globalization;


namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class PreconfiguredRequest
    {
        public static List<GeoMeasure> WeatherHistoryMeasures(string AAAAMMJJHH, WeatherHistoryMetricKey metricKey)
        {
            var query = new QueryRequest(WeatherHistoryDatasource.tableName)
                .Select(new Attribute(WeatherHistoryAttributeKey.LON))
                .Select(new Attribute(WeatherHistoryAttributeKey.LAT))
                .Select(new Attribute(WeatherHistoryAttributeKey.NOM_USUEL))
                .Select(new Metric(metricKey, AggregationKey.AVG))
                .GroupBy(WeatherHistoryAttributeKey.NOM_USUEL)
                .Where(new ExcludeNullFilter(metricKey))
                .Where(new DataFilter(DataFilterMode.INCLUDE, WeatherHistoryAttributeKey.AAAAMMJJHH, AAAAMMJJHH));
            var connector = new SqliteConnector();
            var result = connector.ExecuteQuery(query);

            var res = new List<GeoMeasure>();
            var stationIndex = result.attributeIndex[WeatherHistoryAttributeKey.NOM_USUEL];
            var lonIndex = result.attributeIndex[WeatherHistoryAttributeKey.LON];
            var latIndex = result.attributeIndex[WeatherHistoryAttributeKey.LAT];
            var measureIndex = result.metricIndex[metricKey];
            foreach (var row in result.rows)
            {
                string station = row.attributes[stationIndex].value;
                float lon = float.Parse(row.attributes[lonIndex].value, CultureInfo.InvariantCulture);
                float lat = float.Parse(row.attributes[latIndex].value, CultureInfo.InvariantCulture);
                var measure = row.measures[measureIndex];
                res.Add(new GeoMeasure(new GeoPoint(lat, lon), measure.value.Value));
            }
            return res;
        }
    }
}
