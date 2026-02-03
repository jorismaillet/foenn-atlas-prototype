using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL.CSV;

namespace Assets.Scripts.Foenn.Engine.Inputs.InMemory
{
    public class CSVInputProvider : InMemoryProvider
    {
        private CSVLoader loader;
        private int year;
        private string csvText;
        private int department;
        private UnityEngine.TextAsset textAsset;

        public CSVInputProvider(int department, int year)
        {
            this.department = department;
            this.year = year;
        }

        public override void OpenFile()
        {
            var fileName = WeatherHistoryDataLocator.WeatherFileName(department, year);
            this.textAsset = UnityEngine.Resources.Load<UnityEngine.TextAsset>(fileName);
        }

        public override void Initialize(QueryRequest request)
        {
            this.loader = new CSVLoader();
            this.csvText = textAsset.text;
        }

        public override QueryResult Execute()
        {
            var dateset = loader.Extract(csvText, year);
            throw new System.NotImplementedException("CSV Engine not implemented yet");
        }

        public override void CloseFile()
        {
            UnityEngine.Resources.UnloadAsset(textAsset);
        }
    }
}