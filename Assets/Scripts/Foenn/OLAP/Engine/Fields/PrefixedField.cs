using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.Engine.OLAP.Metrics;
using Assets.Scripts.Foenn.ETL.Models;
using Assets.Scripts.Foenn.OLAP.Engine.Sql;
using System.Data;

namespace Assets.Scripts.Foenn.Engine.Sql
{
    public class PrefixedField : IDataField
    {
        public ITable table;
        public Field field;

        public PrefixedField(ITable table, Field field) {
            this.table = table;
            this.field = field;
        }

        public DbType dbType => field.dbType;

        public string ToSql() {
            return $"\"{table.Name}\".{field.ToSql()}";
        }
    }
}
