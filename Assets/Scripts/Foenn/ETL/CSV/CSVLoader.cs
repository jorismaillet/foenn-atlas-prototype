using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.CSV
{
    public class CSVLoader
    {
        public static char[] STRING_SPLIT = { ';' };
        private const string DECIMAL_SPLIT = ".";

        public CSVLoader()
        {
        }

        public Dataset Extract(string text, int year)
        {
            var reader = new StringReader(text);

            var line = reader.ReadLine();

            var header = line
                .Split(STRING_SPLIT)
                .ToList();

            var result = new Dataset(header, Transform(reader));
            reader.Close();
            return result;
        }

        private List<List<string>> Transform(StringReader reader)
        {
            var res = new List<List<string>>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                res.Add(line.Split(STRING_SPLIT).ToList());
            }
            return res;
        }
    }
}