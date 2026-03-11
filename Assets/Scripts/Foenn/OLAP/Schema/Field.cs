namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using System.Data;

    public class Field : IDataField
    {
        public string name;

        public DbType dbType { get; }

        public ColumnType columnType;

        public Field(string name, DbType type, ColumnType columnType)
        {
            this.name = name;
            this.dbType = type;
            this.columnType = columnType;
        }

        public string ToSql()
        {
            return $"\"{name}\"";
        }
    }
}
