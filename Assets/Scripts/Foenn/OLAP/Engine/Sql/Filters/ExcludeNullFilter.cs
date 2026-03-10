using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    public class ExcludeNullFilter : Filter
    {
        public ExcludeNullFilter(PrefixedField column) : base(column)
        {

        }

        public override string ToSql() {
            return $"{filteredField.ToSql()} IS NOT NULL";
        }
    }
}