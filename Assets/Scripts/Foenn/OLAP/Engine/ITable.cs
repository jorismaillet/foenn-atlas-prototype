using Assets.Scripts.Foenn.Atlas.Datasets.Common;
using Assets.Scripts.Foenn.ETL.Models;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Datasets
{
    public interface ITable
    {
        string Name {  get; }
        PrimaryKey PrimaryKey { get; }
        List<IndexDefinition> Indexes { get; }
        List<Field> Columns { get; }
    }
}
