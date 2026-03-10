using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Unity;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryResult
    {
        public List<Row> rows;
        private System.Action<Row, object>[] columnParsers;
        private List<System.Action<Row, object[]>> lineParsers = new List<System.Action<Row, object[]>>();

        public QueryResult(string[] rawHeaders)
        {
            this.rows = new List<Row>();
            this.columnParsers = new System.Action<Row, object>[rawHeaders.Length];
            var geoHeaderIndexes = new Dictionary<WeatherHistoryGeoAttributeKey, int>();
            for (int i = 0; i < rawHeaders.Length; i++) 
            {
                var header = rawHeaders[i];
                if (System.Enum.TryParse<WeatherHistoryTimeAttributeKey>(header, out var timeAttributeKey))
                {
                    AddTimeDimensionParser(i);
                }
                else if (System.Enum.TryParse<WeatherHistoryGeoAttributeKey>(header, out var geoAttributeKey))
                {
                    geoHeaderIndexes[geoAttributeKey] = i;
                }
                else if (System.Enum.TryParse<WeatherHistoryAttributeKey>(header, out var attributeKey))
                {
                    AddAttributeParser(i, attributeKey.ToString());
                }
                // Metric without aggregation
                else if (System.Enum.TryParse<WeatherHistoryMetricKey>(header, out var metricKey))
                {
                    AddMeasureParser(i, new Metric(metricKey.ToString(), AggregationKey.D_COUNT));
                }
                // Metric with aggregation
                else if (TryParseMetric(header, out var metric))
                {
                    AddMeasureParser(i, metric);
                }
                else
                {
                    throw new System.Exception($"Unknown header {header}");
                }
            }
            if(geoHeaderIndexes.Count == 2)
            {
                AddGeoDimensionParser(geoHeaderIndexes);
            }
        }

        private bool TryParseMetric(object value, out Metric parsedValue)
        {
            var match = new Regex(@"^(\w+)\(\""(.+)\""\)$").Match((string)value);
            if (match.Success)
            {
                parsedValue = new Metric(
                    match.Groups[2].Value,
                    System.Enum.Parse<AggregationKey>(match.Groups[1].Value)
                );
                return true;
            }
            parsedValue = null;
            return false;
        }

        private void AddMeasureParser(int index, Metric metric)
        {
            columnParsers[index] = (Row row, object value) => AddMeasure(row, metric, value);
        }

        private void AddGeoDimensionParser(Dictionary<WeatherHistoryGeoAttributeKey, int> geoAttributeIndexes)
        {
            lineParsers.Add((Row row, object[] rawLine) => {
                row.geo = new GeoField(new GeoPoint(
                    float.Parse((string)rawLine[geoAttributeIndexes[WeatherHistoryGeoAttributeKey.LAT]], CultureInfo.InvariantCulture),
                    float.Parse((string)rawLine[geoAttributeIndexes[WeatherHistoryGeoAttributeKey.LON]], CultureInfo.InvariantCulture)
                ));
            });
        }

        private void AddTimeDimensionParser(int index)
        {
            columnParsers[index] = (Row row, object value) => AddTimeDimension(row, (string)value);
        }

        private void AddAttributeParser(int index, string attribute)
        {
            columnParsers[index] = (Row row, object value) => ParseIfExists(value, (strValue) => AddAttributeValue(row, attribute, strValue));
        }

        private void ParseIfExists(object value, System.Action<string> existsParser)
        {
            var strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                existsParser.Invoke(strValue);
            }
        }

        private void AddMeasure(Row row, Metric metric, object value)
        {
            if (value == null || value is System.DBNull) return;

            float fval;
            switch (value)
            {
                case float f:
                    fval = f;
                    break;
                case double d:
                    fval = (float)d;
                    break;
                case decimal m:
                    fval = (float)m;
                    break;
                case int i:
                    fval = i;
                    break;
                case long l:
                    fval = l;
                    break;
                case short s:
                    fval = s;
                    break;
                case string str:
                    fval = float.Parse(str, CultureInfo.InvariantCulture);
                    break;
                default:
                    fval = System.Convert.ToSingle(value, CultureInfo.InvariantCulture);
                    break;
            }

            row.measures.Add(metric.name, new Measure(metric, fval));
        }

        private void AddTimeDimension(Row row, string value)
        {
            row.time = TimeField.AAAAMMJJHH(value);
        }

        private void AddAttributeValue(Row row, string attribute, string value) { 
            row.attributes.Add(attribute, new AttributeValue(attribute, (string)value));
        }

        public void ParseLine(object[] rawLine)
        {
            Row row = new Row();
            for (int i = 0; i < rawLine.Length; i++)
            {
                columnParsers[i]?.Invoke(row, rawLine[i]);
            }
            lineParsers.ForEach(parser => parser.Invoke(row, rawLine));
            rows.Add(row);
        }
    }
}