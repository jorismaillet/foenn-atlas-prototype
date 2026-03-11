using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.OLAP.Engine.Result;
using Assets.Scripts.OLAP.Schema;

namespace Assets.Scripts.OLAP.Engine
{
    public class QueryResult
    {
        public List<Row> rows;

        private Action<Row, object>[] columnParsers;

        private List<Action<Row, object[]>> lineParsers = new List<Action<Row, object[]>>();

        static readonly Regex AggregationRegex = new Regex(
            @"^(?<agg>[A-Za-z_][A-Za-z0-9_]*)\s*\(\s*(?<expr>.+?)\s*\)$",
            RegexOptions.Compiled
        );

        static readonly Regex ColumnRegex = new Regex(
            @"^(?:(?:""(?<table>[^""]+)""|(?<table>[A-Za-z_][A-Za-z0-9_]*))\s*\.\s*)?(?:""(?<col>[^""]+)""|(?<col>[A-Za-z_][A-Za-z0-9_]*))(?:\s+AS\s+(?:""(?<alias>[^""]+)""|(?<alias>[A-Za-z_][A-Za-z0-9_]*)))?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public QueryResult(string[] rawHeaders, List<Field> selectedColumns)
        {
            this.rows = new List<Row>();
            this.columnParsers = new System.Action<Row, object>[rawHeaders.Length];
            var geoHeaderIndexes = new Dictionary<string, int>();
            for (int i = 0; i < rawHeaders.Length; i++)
            {
                var rawHeader = rawHeaders[i];

                if (!TryParseResultHeader(rawHeader, out var tableName, out var columnName, out var aggregationValue))
                    throw new System.Exception($"Unknown header {rawHeader}");

                var column = selectedColumns.Find(c => c.ToSql().Equals(rawHeader, StringComparison.OrdinalIgnoreCase));
                if (column != null)
                {
                    AddValueParser(i, column);
                    if (columnName == "lon")
                    {
                        geoHeaderIndexes["lon"] = i;
                    }
                    else if (columnName == "lat")
                    {
                        geoHeaderIndexes["lat"] = i;
                    }
                }
            }
            if (geoHeaderIndexes.Count == 2)
            {
                AddGeoDimensionParser(geoHeaderIndexes);
            }
        }

        static bool TryParseResultHeader(string rawHeader, out string tableName, out string columnName, out string aggregationValue)
        {
            tableName = null;
            columnName = null;
            aggregationValue = null;

            if (string.IsNullOrWhiteSpace(rawHeader))
                return false;

            string s = rawHeader.Trim();

            var mAgg = AggregationRegex.Match(s);
            if (mAgg.Success)
            {
                aggregationValue = mAgg.Groups["agg"].Value;
                s = mAgg.Groups["expr"].Value.Trim();

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

        private void AddGeoDimensionParser(Dictionary<string, int> geoAttributeIndexes)
        {
            lineParsers.Add((Row row, object[] rawLine) =>
            {
                row.geo = new GeoPoint(
                    float.Parse((string)rawLine[geoAttributeIndexes["lat"]], CultureInfo.InvariantCulture),
                    float.Parse((string)rawLine[geoAttributeIndexes["lon"]], CultureInfo.InvariantCulture)
                );
            });
        }

        private void AddValueParser(int index, Field field)
        {
            columnParsers[index] = (Row row, object value) =>
            {
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
