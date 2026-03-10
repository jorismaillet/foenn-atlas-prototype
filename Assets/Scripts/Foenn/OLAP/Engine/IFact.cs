namespace Assets.Scripts.Foenn.Datasets
{
    using Assets.Scripts.Foenn.ETL.Dimensions;
    using System;
    using System.Collections.Generic;

    public interface IFact : ITable
    {
        public List<IDimension> Dimensions => throw new NotImplementedException();
    }
}
