namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using System.Collections.Generic;

    public interface IFact : ITable
    {
        List<IDimension> Dimensions { get; }
    }
}
