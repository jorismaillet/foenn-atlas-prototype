using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.OLAP.Schema.Tables
{
    public abstract class Table
    {
        public readonly string Name;

        public readonly Field PrimaryKey;

        public readonly List<IndexDefinition> Indexes;

        public readonly List<FieldMap> Mappings;

        public readonly List<Field> References;

        public virtual IEnumerable<Field> Columns => Mappings.Select(m => m.targetField).Prepend(PrimaryKey).Concat(References);

        public Table(string name)
        {
            this.Name = name;
            PrimaryKey = Field.PK(Name);
            Indexes = new List<IndexDefinition>();
            Mappings = new List<FieldMap>();
            References = new List<Field>();
        }
    }
}
