using Assets.Scripts.Foenn.Engine.Sql;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using System;

namespace Assets.Scripts.Foenn.Engine.OLAP.Filters
{
    public class RangeFilter : Filter
    {
        public int minValue, maxValue;

        public RangeFilter(PrefixedField column, int minValue, int maxValue) : base(column) {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override string ToSql() {
            return $"{filteredField.ToSql()} >= {minValue} AND {filteredField.ToSql()} <= {maxValue}";
        }
    }
}