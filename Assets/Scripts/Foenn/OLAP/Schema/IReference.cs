namespace Assets.Scripts.Foenn.Atlas.Datasets.Common
{
    using Assets.Scripts.Foenn.Datasets;
    using Assets.Scripts.Foenn.ETL.Models;

    public class TableReference
    {
        public Field ReferenceField { get; }

        public ITable Table { get; }
    }
}
