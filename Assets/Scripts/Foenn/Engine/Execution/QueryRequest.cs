using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.Engine.Sql.Clauses;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Foenn.Engine.Execution
{
    public class QueryRequest
    {
        public List<Metric> selectedMetrics = new List<Metric>();
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

        public QueryRequest GroupBy(params WeatherHistoryAttributeKey[] attributes)
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
    }
}