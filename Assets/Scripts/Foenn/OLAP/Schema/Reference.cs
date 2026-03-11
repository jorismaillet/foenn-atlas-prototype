namespace Assets.Scripts.Foenn.OLAP.Schema
{
    public class Reference : Field
    {
        public ITable referencedTable;

        public Reference(ITable table, Field referenceField) : base(referenceField.name, table.PrimaryKey.dbType, table.PrimaryKey.columnType)
        {
            referencedTable = table;
        }
    }
}
