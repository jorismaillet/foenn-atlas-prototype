using Assets.Scripts.Foenn.Atlas.Datasets.Common;
using Assets.Scripts.Foenn.ETL.Models;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.Datasets
{
    public interface ITable
    {
        string Name {  get; }
        List<IndexDefinition> Indexes { get; }
        List<Datafield> Columns { get; }
        List<Reference> References { get; }
    }
}
