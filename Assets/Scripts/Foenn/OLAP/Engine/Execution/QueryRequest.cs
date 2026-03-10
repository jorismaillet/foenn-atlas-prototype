using Assets.Scripts.Foenn.Atlas.Datasets.Common;
using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.OLAP.Dimensions.Attributes;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.Engine.Sql.Clauses;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Unity;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryRequest
    {
        public List<PrefixedField> selectedColumns = new List<PrefixedField>();
        public List<ITable> selectedTables = new List<ITable>();
        public List<PrefixedField> groups = new List<PrefixedField>();
        public List<JoinDefinition> joins = new List<JoinDefinition>();
        public List<Filter> filters = new List<Filter>();
        public ITable from;

        public QueryRequest(ITable from)
        {
            this.from = from;
        }

        public QueryRequest Select(ITable table, params Field[] fields) {
            return Select(null, table, fields);
        }

        public QueryRequest Select(ITable table, Field field, params AggregationKey[] aggregations) {
            foreach (var aggregation in aggregations) {
                Select(aggregation, table, field);
            }
            return this;
        }

        public QueryRequest Select(params PrefixedField[] tableColumns) {
            selectedColumns.AddRange(tableColumns);
            return this;
        }

        public QueryRequest Select(AggregationKey? aggregation, ITable table, params Field[] fields) {
            if (fields.Any()) {
                foreach (var field in fields) {
                    selectedColumns.Add(new PrefixedField(table, field, aggregation));
                }
            }
            else {
                selectedTables.Add(table);
            }
            return this;
        }

        public QueryRequest GroupBy(ITable table, params Field[] fields) {
            foreach (var field in fields) {
                groups.Add(new PrefixedField(table, field));
            }
            return this;
        }

        public QueryRequest Join(ITable leftTable, Reference reference, JoinType joinType) {
            joins.Add(new JoinDefinition(new PrefixedField(leftTable, reference), new PrefixedField(reference.referencedTable, reference.referencedTable.PrimaryKey), joinType));
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
            sql.Append(new SqlSelect(selectedTables, selectedColumns).clause);
            sql.Append(new SqlFrom(from).clause);
            sql.Append(new SqlJoin(joins).clause);
            sql.Append(new SqlWhere(filters).clause);
            sql.Append(new SqlGroupBy(groups).clause);
            return sql.ToString();
        }

        public QueryResult Execute(SqliteConnection connection) {
            var sql = ToSql();
            UnityEngine.Debug.Log($"Executing query : {sql}");
            var queryTime = new Stopwatch();
            queryTime.Start();
            using (var reader = SqliteHelper.ExecuteReader(connection, sql)) {
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