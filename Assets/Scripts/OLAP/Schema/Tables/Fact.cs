using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.OLAP.Schema.Tables
{
    public abstract class Fact : ITable
    {
        public abstract string Name { get; }

        public Field PrimaryKey => Field.PK(Name);

        public abstract List<IndexDefinition> Indexes { get; }

        public abstract List<FieldMap> Mappings { get; }

        public abstract List<Field> References { get; }

        public List<Field> Columns => Mappings.Select(m => m.targetField).Prepend(PrimaryKey).Concat(References).Distinct().ToList();

        public List<Dimension> dimensions;

        protected Fact(List<Dimension> dimensions)
        {
            this.dimensions = dimensions;
        }
    }
}
