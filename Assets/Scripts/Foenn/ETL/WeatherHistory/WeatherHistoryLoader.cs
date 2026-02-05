using Assets.Scripts.Foenn.Engine.Inputs.Databases;
using Assets.Scripts.Foenn.ETL.WeatherHistory;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.CSV
{
    public class WeatherHistoryLoader
    {
        public static char[] STRING_SPLIT = { ';' };
        private const string DECIMAL_SPLIT = ".";

        private string csvText;

        public WeatherHistoryLoader(int department, int year)
        {
            var fileName = WeatherHistoryDataLocator.WeatherFileName(department, year);
            var textAsset = UnityEngine.Resources.Load<UnityEngine.TextAsset>(fileName);
            var csvText = textAsset.text;
        }

        public Dataset Extract()
        {
            var reader = new StringReader(csvText);
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

        public void Load(Dataset dataset, SqlConnector connector)
        {
            var columns = string.Join(", ", dataset.headers);
            connector.OpenSession();
            dataset.lines.ForEach(line =>
            {
                var values = string.Join(", ", dataset.lines[i]);
                var sql = $"INSERT INTO \"{WeatherHistoryMetaData.table_name}\" ({columns}) VALUES ({values})";
                connector.ExecuteOperation(sql);
            });
            connector.CloseSession();
        }
    }
}