namespace Assets.Scripts.Foenn.OLAP.Fields
{
    using Assets.Scripts.Foenn.OLAP.Query;
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Data;

    public class AggregatedField : IDataField
    {
        public Field field;

        public AggregationKey aggregation;

        public AggregatedField(Field field, AggregationKey aggregation)
        {
            this.field = field;
            this.aggregation = aggregation;
        }

        public DbType dbType => field.dbType;

        public string ToSql()
        {
            return $"{aggregation}({field.ToSql()})";
        }
    }
}
