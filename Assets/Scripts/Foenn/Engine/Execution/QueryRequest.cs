using Assets.Scripts.Foenn.Engine.Attributes;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Filters;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.Engine.Sql.Clauses;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using System.Collections.Generic;
using System.Text;
using UnityEditor.PackageManager.Requests;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryRequest
    {
        public List<Metric> selectedMetrics = new List<Metric>();
        public List<AttributeKey> groups = new List<AttributeKey>();
        public List<Filter> filters = new List<Filter>();

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
            sql.Append(new SqlFrom("weather_data", dialect).clause);
            sql.Append(new SqlWhere(filters, dialect).clause);
            sql.Append(new SqlGroupBy(groups, dialect).clause);
            sql.Append(dialect.EndOfLine());
            return sql.ToString();
        }
    }
}