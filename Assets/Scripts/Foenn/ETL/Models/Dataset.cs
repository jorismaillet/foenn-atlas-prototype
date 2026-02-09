using System.Collections.Generic;

namespace Assets.Scripts.Foenn.ETL
{
    public class Dataset
    {
        public List<Datafield> fields = new List<Datafield>();
        public List<List<string>> lines = new List<List<string>>();
    }
}