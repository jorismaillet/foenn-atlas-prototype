using Assets.Scripts.Foenn.ETL.Dimensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Foenn.Datasets
{
    public interface IFact : ITable
    {
        public List<IDimension> Dimensions => throw new NotImplementedException();
    }
}
