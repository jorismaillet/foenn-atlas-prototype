using System.Collections.Generic;
using Assets.Scripts.OLAP.Schema.Fields;

namespace Assets.Scripts.OLAP.Schema.Tables
{
    public interface ITable
    {
        public string Name { get; }
        public Field PrimaryKey { get; }
        public List<IndexDefinition> Indexes { get; }
        public List<FieldMap> Mappings { get; }
        public List<Field> References { get; }
        public List<Field> Columns { get; }
    }
}
