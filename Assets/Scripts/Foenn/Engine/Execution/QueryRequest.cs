using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Inputs.Databases;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.Sql.Clauses;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryRequest
    {
        public List<Metric> selectedMetrics = new List<Metric>();
        public List<AttributeKey> groups = new List<AttributeKey>();
        public List<Filter> filters = new List<Filter>();
        public string from;

        public QueryRequest(string tableName) {
            this.from = tableName;
        }

        public QueryRequest Select(params Metric[] metrics)
        {
            this.selectedMetrics.AddRange(metrics);
            return this;
        }

        public QueryRequest GroupBy(params AttributeKey[] attributes)
        {
            this.groups.AddRange(attributes);
            return this;
        }

        public QueryRequest Where(params Filter[] filters)
        {
            this.filters.AddRange(filters);
            return this;
        }

        public string ToSql(ISqlDialect dialect)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(new SqlSelect(selectedMetrics, dialect).clause);
            sql.Append(new SqlFrom(from, dialect).clause);
            sql.Append(new SqlWhere(filters, dialect).clause);
            sql.Append(new SqlGroupBy(groups, dialect).clause);
            sql.Append(dialect.EndOfLine());
            return sql.ToString();
        }

        public QueryResult ExecuteOnce(SqlConnector connector)
        {
            connector.OpenSession();
            var res = connector.ExecuteQuery(this);
            connector.CloseSession();
            return res;
        }
    }
}