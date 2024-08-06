using System.Collections.Generic;

namespace Assets.Scripts
{
    public class CSVResult
    {
        public string[] headers;
        public IEnumerable<string> lines;

        public CSVResult(string[] headers, IEnumerable<string> lines)
        {
            this.headers = headers;
            this.lines = lines;
        }
    }
}