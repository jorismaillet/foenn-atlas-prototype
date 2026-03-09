using Assets.Scripts.Foenn.Datasets;
using Assets.Scripts.Foenn.ETL.Models;

namespace Assets.Scripts.Foenn.Atlas.Datasets.Common {
    public class Reference {
        public Reference(Datafield referenceField, ITable table) {
            ReferenceField = referenceField;
            Table = table;
        }

        public Datafield ReferenceField { get; }
        public ITable Table { get; }
    }
}
