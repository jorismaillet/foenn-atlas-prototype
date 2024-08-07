using Assets.Resources.Weathers;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class CSVResult
    {
        public List<WeatherFieldKey> header;
        public List<WeatherRecord> lines;

        public CSVResult(List<WeatherFieldKey> header, List<WeatherRecord> lines)
        {
            this.header = header;
            this.lines = lines;
        }
    }
}