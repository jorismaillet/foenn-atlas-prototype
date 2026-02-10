using Assets.Scripts.Common.Extensions;
using Assets.Scripts.Foenn.Engine.Attributes.AttributeKeys;
using Assets.Scripts.Foenn.Engine.Metrics;
using Assets.Scripts.Foenn.ETL.Extractors;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.CSV
{
    public class CSVExtractor : Extractor
    {
        public static char[] STRING_SPLIT = { ';' };
        private const string DECIMAL_SPLIT = ".";
        private string csvText;

        public CSVExtractor(string fileName)
        {
            var textAsset = UnityEngine.Resources.Load<UnityEngine.TextAsset>(fileName);
            this.csvText = textAsset.text;
        }

        public override void Extract(Dataset dataset)
        {
            var reader = new StringReader(csvText);
            dataset.fields = ExtractFields(reader.ReadLine());
            dataset.lines = ExtractLines(reader);
            reader.Close();
        }
        
        private List<Datafield> ExtractFields(string header)
        {
            return header
                .Split(STRING_SPLIT)
                .Select((field) => new Datafield(field, GetDataType(field)))
                .ToList();
        }

        private Datatype GetDataType(string field)
        {
            if (System.Enum.TryParse<WeatherHistoryAttributeKey>(field, out _))
            {
                return Datatype.STRING;
            }
            else if (System.Enum.TryParse<WeatherHistoryMetricKey>(field, out _))
            {
                return Datatype.FLOAT;
            }
            else
            {
                throw new System.Exception($"Unknown field type for {field}");
            }
        }

        private List<List<string>> ExtractLines(StringReader reader)
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