using Assets.Scripts.Foenn.Engine.OLAP.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlWhere
    {
        public readonly string clause;
        public SqlWhere(List<Filter> filters)
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
                        filteredAttributes = $"({string.Join(", ", df.selectedValues.Select(v => $"\"{v}\""))})";
                    }
                    else
                    {
                        filterOperator = df.mode.Equals(DataFilterMode.INCLUDE) ? "=" : "!=" + " ";
                        filteredAttributes = $"\"{df.selectedValues[0]}\"";
                    }
                    whereParts.Add($"\"{df.filteredAttributeKey}\"{filterOperator}{filteredAttributes}");
                }
                else if (filter is RangeFilter tf)
                {
                    whereParts.Add($"\"{tf.attributeName}\" >= {tf.minValue} AND \"{tf.attributeName}\" <= {tf.maxValue}");
                }
                else if (filter is ExcludeNullFilter enf)
                {
                    whereParts.Add($"\"{enf.metricKey}\" IS NOT NULL");
                }
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