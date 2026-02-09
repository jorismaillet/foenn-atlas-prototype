using Assets.Scripts.Foenn.ETL.Datasources;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.MemoryProfiler;

namespace Assets.Scripts.Foenn.ETL.CSV
{
    public class WeatherHistoryDatasource : Datasource
    {
        public static string tableName = "weather_data";
        public override string IdColumn() => "INSERT_ID";

        public override string TableName() => tableName;
        public override string Identifier(Dictionary<string, int> headerIndexes, List<string> line)
        {
            return line[headerIndexes["NUM_POSTE"]] + line[headerIndexes["AAAAMMJJHH"]];
        }

        public override void Transform(Dataset dataset)
        {
            dataset.fields.Add(new Datafield(IdColumn(), Datatype.STRING));
            var columnIndexes = dataset.fields.Select((f, i) => new { f.name, index = i }).ToDictionary(x => x.name, x => x.index);
            dataset.lines.ForEach(line =>
            {
                line.Add(Identifier(columnIndexes, line));
            });
        }
    }
}