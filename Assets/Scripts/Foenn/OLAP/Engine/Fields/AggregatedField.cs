using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Foenn.OLAP.Engine.Sql;
using System.Data;

namespace Assets.Scripts.Foenn.Engine.Sql
{
    public class AggregatedField : IDataField
    {
        public Field field;
        public AggregationKey aggregation;

        public AggregatedField(Field field, AggregationKey aggregation) {
            this.field = field;
            this.aggregation = aggregation;
        }

        public DbType dbType => field.dbType;

        public string ToSql() {
            return $"{aggregation}({field.ToSql()})";
        }
    }
}
