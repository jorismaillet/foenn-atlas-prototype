using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.OLAP.Schema.Tables
{
    public abstract class Dimension : ITable
    {
        public abstract string Name { get; }
        public abstract Field PrimaryKey { get; }
        public abstract List<IndexDefinition> Indexes { get; }
        public abstract List<FieldMap> Mappings { get; }
        public abstract Field LookupField { get; }
        public abstract SourceField LookupSourceAttribute { get; }
        public List<Field> References => new List<Field>();
        public List<Field> Columns => Mappings.Select(m => m.targetField).Prepend(PrimaryKey).Concat(References).Distinct().ToList();
    }
}
