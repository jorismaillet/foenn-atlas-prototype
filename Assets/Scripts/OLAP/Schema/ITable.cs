using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.OLAP.Schema
{
    public interface ITable
    {
        string name { get; }

        Field PrimaryKey { get; }

        List<IndexDefinition> Indexes { get; }

        List<FieldMap> Mappings { get; }

        List<Field> References => new List<Field>();

        List<Field> Columns => Mappings.Select(m => m.targetField).Prepend(PrimaryKey).Concat(References).Distinct().ToList();
    }
}
