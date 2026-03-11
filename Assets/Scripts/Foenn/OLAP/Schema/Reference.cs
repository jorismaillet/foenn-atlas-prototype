namespace Assets.Scripts.Foenn.OLAP.Schema
{
    public class Reference : Field
    {
        public IDimension referencedDimension;
        public string sourceCsvColumn;

        public Reference(IDimension dimension, Field referenceField, string sourceCsvColumn) 
            : base(referenceField.name, dimension.PrimaryKey.dbType, dimension.PrimaryKey.columnType)
        {
            referencedDimension = dimension;
            this.sourceCsvColumn = sourceCsvColumn;
        }
    }
}
