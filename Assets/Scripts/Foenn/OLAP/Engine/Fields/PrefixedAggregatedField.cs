using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Foenn.OLAP.Engine.Sql;

namespace Assets.Scripts.Foenn.Engine.Sql
{
    public class PrefixedAggregatedField : IDataField
    {
        public ITable table;
        public Field field;
        public AggregationKey aggregation;

        public PrefixedAggregatedField(ITable table, Field field, AggregationKey aggregation) {
            this.table = table;
            this.field = field;
            this.aggregation = aggregation;
        }

        public string ToSql() {
            return $"{aggregation}(\"{table.Name}\".{field.ToSql()})";
        }
    }
}
