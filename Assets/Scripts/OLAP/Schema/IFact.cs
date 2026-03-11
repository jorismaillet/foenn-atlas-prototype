using System.Collections.Generic;

namespace Assets.Scripts.OLAP.Schema
{
    public interface IFact : ITable
    {
        List<IDimension> Dimensions { get; }
    }
}
