namespace Assets.Scripts.Foenn.ETL.Extractors
{
    using System.Collections.Generic;
    using System.IO;

    public class CSVExtractor
    {
        public static char[] STRING_SPLIT = { ';' };

        private const string DECIMAL_SPLIT = ".";

        private string fileName;

        public CSVExtractor(string fileName)
        {
            this.fileName = fileName;
        }

        public string[] ExtractFieldNames()
        {
            using var sr = GetStreamReader();
            var headerStr = sr.ReadLine();
            return headerStr.Split(STRING_SPLIT);
        }

        public IEnumerable<string[]> ExtractValues()
        {
            using var sr = GetStreamReader();
            string line;
            sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line)) continue;
                yield return line.Split(STRING_SPLIT);
            }
        }

        private StreamReader GetStreamReader()
        {
            var path = Path.Combine(UnityEngine.Application.dataPath, "Resources", fileName);
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1 << 20);
            return new StreamReader(fs);
        }
    }
}
