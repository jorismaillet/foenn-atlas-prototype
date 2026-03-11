namespace Assets.Scripts.Foenn.OLAP.Fields
{
    using Assets.Scripts.Foenn.OLAP.Query;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Data;

    public class PrefixedAggregatedField : IDataField
    {
        public ITable table;

        public Field field;

        public AggregationKey aggregation;

        public PrefixedAggregatedField(ITable table, Field field, AggregationKey aggregation)
        {
            this.table = table;
            this.field = field;
            this.aggregation = aggregation;
        }

        public DbType dbType => field.dbType;

        public string ToSql()
        {
            return $"{aggregation}(\"{table.Name}\".{field.ToSql()})";
        }
    }
}
