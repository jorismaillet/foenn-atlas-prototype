using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.OLAP.Engine.Sql;
using Assets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Data;
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

        static readonly Regex AggregationRegex = new Regex(
            @"^(?<agg>[A-Za-z_][A-Za-z0-9_]*)\s*\(\s*(?<expr>.+?)\s*\)$",
            RegexOptions.Compiled
        );

        static readonly Regex ColumnRegex = new Regex(
            @"^(?:(?:""(?<table>[^""]+)""|(?<table>[A-Za-z_][A-Za-z0-9_]*))\s*\.\s*)?(?:""(?<col>[^""]+)""|(?<col>[A-Za-z_][A-Za-z0-9_]*))(?:\s+AS\s+(?:""(?<alias>[^""]+)""|(?<alias>[A-Za-z_][A-Za-z0-9_]*)))?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public QueryResult(string[] rawHeaders, List<IDataField> columns)
        {
            this.rows = new List<Row>();
            this.columnParsers = new System.Action<Row, object>[rawHeaders.Length];
            var geoHeaderIndexes = new Dictionary<WeatherHistoryGeoAttributeKey, int>();
            for (int i = 0; i < rawHeaders.Length; i++) 
            {
                var rawHeader = rawHeaders[i];

                if (!TryParseResultHeader(rawHeader, out var tableName, out var columnName, out var aggregationValue))
                    throw new System.Exception($"Unknown header {rawHeader}");

                var column = columns.Find(c => c.ToSql().Equals(rawHeader, StringComparison.OrdinalIgnoreCase));

                AddValueParser(i, column);

                if (System.Enum.TryParse<WeatherHistoryGeoAttributeKey>(columnName, out var geoAttributeKey))
                {
                    geoHeaderIndexes[geoAttributeKey] = i;
                }
            }
            if(geoHeaderIndexes.Count == 2)
            {
                AddGeoDimensionParser(geoHeaderIndexes);
            }
        }

        // Parses a reader column name/expression and tries to extract:
        // - aggregation: e.g. AVG / MIN / MAX / SUM / D_COUNT
        // - table name: e.g. weather_data (optional)
        // - column name: e.g. T / LAT / temperature
        //
        // Supported examples:
        // - T
        // - "T"
        // - "weather_data"."T"
        // - AVG("T")
        // - AVG("weather_data"."T")
        // - AVG(weather_data.T)
        static bool TryParseResultHeader(string rawHeader, out string tableName, out string columnName, out string aggregationValue)
        {
            tableName = null;
            columnName = null;
            aggregationValue = null;

            if (string.IsNullOrWhiteSpace(rawHeader))
                return false;

            string s = rawHeader.Trim();

            // If the SQL used an explicit alias, SQLite typically returns just the alias as GetName().
            // We still try to parse it as a column identifier.

            var mAgg = AggregationRegex.Match(s);
            if (mAgg.Success)
            {
                aggregationValue = mAgg.Groups["agg"].Value;
                s = mAgg.Groups["expr"].Value.Trim();

                // Handle DISTINCT prefix (kept simple for now).
                const string distinctPrefix = "DISTINCT ";
                if (s.StartsWith(distinctPrefix, StringComparison.OrdinalIgnoreCase))
                    s = s.Substring(distinctPrefix.Length).Trim();
            }

            var mCol = ColumnRegex.Match(s);
            if (!mCol.Success)
                return false;

            tableName = mCol.Groups["table"].Success ? mCol.Groups["table"].Value : null;
            columnName = mCol.Groups["col"].Value;
            return !string.IsNullOrEmpty(columnName);
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

        private void AddValueParser(int index, IDataField field) {
            columnParsers[index] = (Row row, object value) => {
                if (value == null || value is System.DBNull) return;
                row.values[field] = value;
            };
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