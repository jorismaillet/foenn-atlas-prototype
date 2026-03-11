namespace Assets.Scripts.Foenn.OLAP.Query
{
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using Assets.Scripts.Foenn.OLAP.Sql;
    using Assets.Scripts.Unity;
    using Mono.Data.Sqlite;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    public class QueryRequest
    {
        public List<Field> selectedColumns = new List<Field>();
        public List<ITable> selectedTables = new List<ITable>();
        public List<Field> groups = new List<Field>();
        public List<JoinDefinition> joins = new List<JoinDefinition>();
        public List<Filter> filters = new List<Filter>();
        public ITable from;

        public QueryRequest(ITable from)
        {
            this.from = from;
        }

        public QueryRequest Select(params Field[] fields)
        {
            selectedColumns.AddRange(fields);
            return this;
        }

        public QueryRequest Select(ITable table, params Field[] fields)
        {
            foreach (var field in fields)
                selectedColumns.Add(field.Of(table));
            return this;
        }

        public QueryRequest Select(AggregationKey aggregation, params Field[] fields)
        {
            foreach (var field in fields)
                selectedColumns.Add(field.As(aggregation));
            return this;
        }

        public QueryRequest Select(ITable table, Field field, params AggregationKey[] aggregations)
        {
            foreach (var aggregation in aggregations)
                selectedColumns.Add(field.Of(table, aggregation));
            return this;
        }

        public QueryRequest GroupBy(params Field[] fields)
        {
            groups.AddRange(fields);
            return this;
        }

        public QueryRequest Join(ITable leftTable, Field refField, JoinType joinType)
        {
            var leftField = refField.Of(leftTable);
            var rightField = refField.referencedDimension.PrimaryKey.Of(refField.referencedDimension);
            joins.Add(new JoinDefinition(leftField, rightField, joinType));
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

        public QueryResult Execute(SqliteConnection connection)
        {
            var sql = ToSql();
            UnityEngine.Debug.Log($"Executing query : {sql}");
            var queryTime = new Stopwatch();
            queryTime.Start();
            using (var reader = SqliteHelper.ExecuteReader(connection, sql))
            {
                int nbColumns = reader.FieldCount;
                string[] rawHeaders = new string[nbColumns];
                for (int i = 0; i < nbColumns; i++)
                {
                    rawHeaders[i] = reader.GetName(i);
                }
                var res = new QueryResult(rawHeaders, selectedColumns);
                while (reader.Read())
                {
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
