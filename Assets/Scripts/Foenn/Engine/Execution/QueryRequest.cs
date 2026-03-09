using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.Engine.Sql.Clauses;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Unity;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryRequest
    {
        public List<Metric> selectedMetrics = new List<Metric>();
        public List<Attribute> selectedAttributes = new List<Attribute>();
        public List<WeatherHistoryAttributeKey> groups = new List<WeatherHistoryAttributeKey>();
        public List<Filter> filters = new List<Filter>();
        public string from;

        public QueryRequest(string tableName)
        {
            this.from = tableName;
        }

        public QueryRequest Select(params Metric[] metrics)
        {
            this.selectedMetrics.AddRange(metrics);
            return this;
        }

        public QueryRequest Select(params Attribute[] attributes)
        {
            this.selectedAttributes.AddRange(attributes);
            return this;
        }

        public QueryRequest GroupBy(params WeatherHistoryAttributeKey[] attributes)
        {
            foreach (var attribute in attributes)
            {
                groups.Add(attribute);
            }
            return this;
        }

        public QueryRequest Where(params Filter[] filters)
        {
            this.filters.AddRange(filters);
            return this;
        }

        public string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(new SqlSelect(selectedMetrics, selectedAttributes, dialect).clause);
            sql.Append(new SqlFrom(from, dialect).clause);
            sql.Append(new SqlWhere(filters, dialect).clause);
            sql.Append(new SqlGroupBy(groups, dialect).clause);
            sql.Append(dialect.EndOfLine());
            return sql.ToString();
        }

        public QueryResult Execute(SqliteConnection connection) {
            var sql = ToSql();
            UnityEngine.Debug.Log($"Executing query : {sql}");
            var queryTime = new Stopwatch();
            queryTime.Start();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            using (var reader = command.ExecuteReader()) {
                int nbColumns = reader.FieldCount;
                string[] rawHeaders = new string[nbColumns];
                for (int i = 0; i < nbColumns; i++) {
                    rawHeaders[i] = reader.GetName(i);
                }
                var res = new QueryResult(rawHeaders);
                while (reader.Read()) {
                    var row = new object[nbColumns];
                    reader.GetValues(row);
                    res.ParseLine(row);
                }
                queryTime.Stop();
                MainThreadLog.Log($"Query completed in {queryTime.ElapsedMilliseconds}ms");
                return res;
            }
        }
    }
}