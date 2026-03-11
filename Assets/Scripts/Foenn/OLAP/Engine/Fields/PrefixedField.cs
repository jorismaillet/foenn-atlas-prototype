namespace Assets.Scripts.Foenn.OLAP.Fields
{
    using Assets.Scripts.Foenn.OLAP.Schema;
    using System.Data;

    public class PrefixedField : IDataField
    {
        public ITable table;

        public Field field;

        public PrefixedField(ITable table, Field field)
        {
            this.table = table;
            this.field = field;
        }

        public DbType dbType => field.dbType;

        public string ToSql()
        {
            return $"\"{table.Name}\".{field.ToSql()}";
        }
    }
}
