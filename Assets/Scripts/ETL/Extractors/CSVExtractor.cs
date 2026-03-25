using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.ETL.Extractors
{
    public class CSVExtractor
    {
        private const char SEPARATOR = ';';

        private string fileName;

        private string[] _buffer;

        public CSVExtractor(string fileName)
        {
            this.fileName = fileName;
        }

        public string[] ExtractFieldNames()
        {
            using var sr = GetStreamReader();
            var headerStr = sr.ReadLine();
            return headerStr.Split(SEPARATOR);
        }

        public IEnumerable<string[]> ExtractValues()
        {
            using var sr = GetStreamReader();
            string line;
            sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line)) continue;
                SplitLine(line);
                yield return _buffer;
            }
        }

        private void SplitLine(string line)
        {
            int count = 1;
            for (int i = 0; i < line.Length; i++)
                if (line[i] == SEPARATOR) count++;

            if (_buffer == null || _buffer.Length != count)
                _buffer = new string[count];

            int start = 0, field = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == SEPARATOR)
                {
                    _buffer[field++] = line.Substring(start, i - start);
                    start = i + 1;
                }
            }
            _buffer[field] = line.Substring(start);
        }

        private StreamReader GetStreamReader()
        {
            var path = Path.Combine(UnityEngine.Application.dataPath, "Resources", fileName);
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1 << 20);
            return new StreamReader(fs);
        }
    }
}
