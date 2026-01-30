using Assets.Resources.Weathers;
using System.Collections.Generic;

namespace Assets.Scripts.Foenn.ETL
{
    public class Dataset
    {
        public List<DataLine> lines;

        public Dataset(List<DataLine> lines)
        {
            this.lines = lines;
        }
    }
}