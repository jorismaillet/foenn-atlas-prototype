using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL.CSV;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Inputs.InMemory
{
    public class CSVInputProvider : InMemoryProvider
    {
        private CSVLoader loader;
        private int year;
        private string csvText;

        public CSVInputProvider(int department, int year) {
            this.loader = new CSVLoader();

            var fileName = WeatherHistoryLoader.WeatherFileName(department, year);
            var textAsset = Resources.Load<TextAsset>(fileName);
            this.csvText = textAsset.text;
            this.year = year;
            Resources.UnloadAsset(textAsset);
        }

        public void Initialize(QueryRequest request)
        {
            return loader.Extract(csvText, year);
        }
    }
}