namespace Assets.Scripts.Foenn.Atlas.Datasets.Common
{
    using Assets.Scripts.Foenn.Datasets;
    using Assets.Scripts.Foenn.ETL.Models;

    public class Reference : Field
    {
        public ITable referencedTable;

        public Reference(ITable table, Field referenceField) : base(referenceField.name, table.PrimaryKey.dbType, table.PrimaryKey.columnType)
        {
            referencedTable = table;
        }
    }
}
