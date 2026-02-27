using Assets.Scripts.Foenn.Engine.OLAP.Dimensions;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryResult
    {
        public List<Row> rows;
        private List<System.Action<Row, object>> columnParser = new List<System.Action<Row, object>>();

        public QueryResult(string[] rawHeaders)
        {
            this.rows = new List<Row>();
            foreach (var header in rawHeaders)
            {
                if (System.Enum.TryParse<WeatherHistoryAttributeKey>(header, out var attributeKey))
                {
                    AddDimensionParser(new Attribute(attributeKey));
                }
                // Metric without aggregation
                else if (System.Enum.TryParse<WeatherHistoryMetricKey>(header, out var metricKey))
                {
                    AddMeasureParser(new Metric(metricKey, AggregationKey.D_COUNT));
                }
                // Metric with aggregation
                else if(TryParseMetric(header, out var metric))
                {
                    AddMeasureParser(metric);
                }
                else
                {
                    throw new System.Exception($"Unknown header {header}");
                }
            }
        }

        private bool TryParseMetric(object value, out Metric parsedValue)
        {
            var match = new Regex(@"^(\w+)\(\""(.+)\""\)$").Match((string)value);
            if (match.Success)
            {
                parsedValue = new Metric(
                    System.Enum.Parse<WeatherHistoryMetricKey>(match.Groups[2].Value),
                    System.Enum.Parse<AggregationKey>(match.Groups[1].Value)
                );
                return true;
            }
            parsedValue = null;
            return false;
        }

        private void AddMeasureParser(Metric metric)
        {
            columnParser.Add((Row row, object value) => AddMeasure(row, metric, value));
        }

        private void ParseIfExists(Row row, Attribute attribute, object value, System.Action<Row, Attribute, string> parser)
        {
            var strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                parser(row, attribute, strValue);
            }
        }

        private void AddDimensionParser(Attribute attribute) {
            if (attribute.key.Equals(WeatherHistoryAttributeKey.AAAAMMJJHH))
            {
                columnParser.Add((Row row, object value) => ParseIfExists(row, attribute, value, AddTimeDimension));
            }
            else if (attribute.key.Equals(WeatherHistoryAttributeKey.NUM_POSTE))
            {
                columnParser.Add((Row row, object value) => ParseIfExists(row, attribute, value, AddGeoDimension));
            }
            else
            {
                columnParser.Add((Row row, object value) => ParseIfExists(row, attribute, value, AddAttributeValue));
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

            row.measures.Add(new Measure(metric, fval));
        }

        private void AddTimeDimension(Row row, Attribute attribute, string value)
        {
            row.time = TimeDimension.AAAAMMJJHH((string)value);
        }

        private void AddGeoDimension(Row row, Attribute attribute, string value)
        {
            row.geo = new GeoDimension() { numPost = value.ToString() };
        }

        private void AddAttributeValue(Row row, Attribute attribute, string value) { 
            row.attributes.Add(new AttributeValue(attribute, (string)value));
        }

        public void ParseLine(object[] rawLine)
        {
            Row row = new Row();
            for (int i = 0; i < rawLine.Length; i++)
            {
                columnParser[i].Invoke(row, rawLine[i]);
            }
            rows.Add(row);
        }
    }
}