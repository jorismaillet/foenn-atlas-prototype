namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using System.Collections.Generic;

    public interface ITable
    {
        string Name { get; }

        PrimaryKey PrimaryKey { get; }

        List<IndexDefinition> Indexes { get; }

        List<Field> Columns { get; }
    }
}
