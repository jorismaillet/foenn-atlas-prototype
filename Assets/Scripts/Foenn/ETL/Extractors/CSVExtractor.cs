using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Assets.Scripts.Foenn.ETL.Extractors
{
    public class CSVExtractor : Extractor
    {
        public static char[] STRING_SPLIT = { ';' };
        private const string DECIMAL_SPLIT = ".";
        private string fileName;

        public CSVExtractor(string fileName)
        {
            this.fileName = fileName;
        }

        public override List<Datafield> ExtractHeaders()
        {
            using var sr = GetStreamReader();
            var headerStr = sr.ReadLine();
            return headerStr
                .Split(STRING_SPLIT)
                .Select((field) => new Datafield(field, GetDataType(field)))
                .ToList();
        }

        public override IEnumerable<string[]> ExtractContent(int headersCount)
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

        public override string ExtractionID()
        {
            return fileName;
        }

        public static int SplitLine(ReadOnlySpan<char> line, char sep, Span<Range> ranges)
        {
            int count = 0;
            int start = 0;

            for (int i = 0; i <= line.Length; i++)
            {
                if (i == line.Length || line[i] == sep)
                {
                    ranges[count++] = start..i;
                    start = i + 1;
                }
            }
            return count;
        }
    }
}