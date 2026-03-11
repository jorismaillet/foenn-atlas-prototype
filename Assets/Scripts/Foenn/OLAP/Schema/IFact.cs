namespace Assets.Scripts.Foenn.OLAP.Schema
{
    using System;
    using System.Collections.Generic;

    public interface IFact : ITable
    {
        public List<IDimension> Dimensions { get; }
    }
}
