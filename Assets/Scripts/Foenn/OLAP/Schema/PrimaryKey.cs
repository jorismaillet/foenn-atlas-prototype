namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using System.Data;

    public class PrimaryKey : Field
    {
        public IndexDefinition indexDefinition;

        public bool autoIncrement;

        public PrimaryKey(string name, DbType dbType, ColumnType columnType, bool autoIncrement) : base(name, dbType, columnType)
        {
            this.autoIncrement = autoIncrement;
            this.indexDefinition = new IndexDefinition(true, this);
        }
    }
}
