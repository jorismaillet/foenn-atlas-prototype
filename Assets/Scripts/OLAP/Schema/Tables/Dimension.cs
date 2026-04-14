using System.Linq;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.OLAP.Schema.Tables
{
    public abstract class Dimension : Table
    {
        public abstract Field LookupField { get; }

        public readonly SourceField LookupSourceAttribute;

        public Dimension(string name, SourceField LookupSourceAttribute) : base(name)
        {
            this.LookupSourceAttribute = LookupSourceAttribute;
        }

        public FieldMap LookupFieldMap => Mappings.First(m => m.targetField == LookupField);
    }
}
