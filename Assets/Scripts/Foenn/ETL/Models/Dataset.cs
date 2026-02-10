using System.Collections.Generic;

namespace Assets.Scripts.Foenn.ETL.Models
{
    public class Dataset
    {
        public List<Datafield> fields = new List<Datafield>();
        public List<List<string>> lines = new List<List<string>>();
    }
}