namespace Assets.Scripts.Foenn.Engine.Sql
{
    using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
    using Assets.Scripts.Foenn.ETL.Models;
    using Assets.Scripts.Foenn.OLAP.Engine.Sql;
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
