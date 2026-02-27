using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using Assets.Scripts.Foenn.Engine.Sql.Dialects;
using Assets.Scripts.Foenn.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlWhere
    {
        public readonly string clause;
        public SqlWhere(List<Filter> filters, ISqlDialect dialect)
        {
            var whereParts = new List<string>();
            foreach (var filter in filters)
            {
                if (filter is DataFilter df)
                {
                    string filterOperator = null;
                    string filteredAttributes = null;
                    if (df.selectedValues.Count > 1)
                    {
                        filterOperator = df.mode.Equals(DataFilterMode.INCLUDE) ? " IN" : " NOT IN";
                        filteredAttributes = $"({string.Join(", ", df.selectedValues.Select(v => dialect.QuoteIdent(v)))})";
                    }
                    else
                    {
                        filterOperator = df.mode.Equals(DataFilterMode.INCLUDE) ? dialect.Equals() : dialect.Different() + " ";
                        filteredAttributes = dialect.QuoteIdent(df.selectedValues[0]);
                    }
                    whereParts.Add($"{dialect.QuoteIdent(df.filteredAttributeKey.ToString())}{filterOperator}{filteredAttributes}");
                }
                else if (filter is TimeRangeFilter tf)
                {
                    whereParts.Add($"AAAAMMJJHH >= {TimeUtils.ToString(tf.startTime)} AND AAAAMMJJHH <= {TimeUtils.ToString(tf.endTime)}");
                }
                else if (filter is ExcludeNullFilter enf)
                {
                    whereParts.Add($"{dialect.QuoteIdent(enf.metricKey.ToString())} IS NOT NULL");
                }
                // TODO GeoFilter
                else
                {
                    throw new System.NotImplementedException($"Filter type {filter.GetType().Name} not supported in SQL generation yet.");
                }
            }
            if (whereParts.Count > 0)
                clause = " WHERE " + string.Join(" AND ", whereParts);
            else
                clause = string.Empty;
        }
    }
}