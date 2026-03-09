using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Engine.Sql.Clauses
{
    public class SqlGroupBy
    {
        public readonly string clause;
        public SqlGroupBy(List<WeatherHistoryAttributeKey> groups)
        {
            var groupByParts = new List<string>();
            foreach (var attr in groups)
            {
                groupByParts.Add($"\"{attr}\"");
            }
            clause = groupByParts.Count > 0
                ? " GROUP BY " + string.Join(", ", groupByParts)
                : string.Empty;
        }
    }
}