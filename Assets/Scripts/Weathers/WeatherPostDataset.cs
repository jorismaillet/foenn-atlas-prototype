using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Resources.Weathers
{
    public class WeatherPostDataset
    {
        public string post;
        public int department;
        public IEnumerable<WeatherRecord> records;

        public WeatherPostDataset(string post, int department, IEnumerable<WeatherRecord> records)
        {
            this.post = post;
            this.department = department;
            this.records = records;
        }

        public WeatherPostDataset(string city, int year, string fileText, List<WeatherRecordFieldKey> keysToLoad)
        {
            var csv = new CSVLoader().LoadCSV(fileText, year, keysToLoad);
            records = csv.lines.Where(line => line.Get(WeatherRecordFieldKey.NOM_USUEL).IndexOf(city, StringComparison.OrdinalIgnoreCase) >= 0);
            this.post = records.First().Get(WeatherRecordFieldKey.NOM_USUEL);
        }
    }
}