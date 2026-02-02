using System.Collections.Generic;

namespace Assets.Scripts.Foenn.ETL
{
    public class Dataset
    {
        public List<string> headers;
        public List<List<string>> lines;

        public Dataset(List<string> headers, List<List<string>> lines)
        {
            this.headers = headers;
            this.lines = lines;
        }
    }
}