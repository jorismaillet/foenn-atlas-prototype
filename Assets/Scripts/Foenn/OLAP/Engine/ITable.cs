namespace Assets.Scripts.Foenn.Datasets
{
    using Assets.Scripts.Foenn.ETL.Models;
    using System.Collections.Generic;

    public interface ITable
    {
        string Name { get; }

        PrimaryKey PrimaryKey { get; }

        List<IndexDefinition> Indexes { get; }

        List<Field> Columns { get; }
    }
}
