using System.Collections.Generic;

namespace Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory
{
    public class WeatherHistoryDatasource : Datasource
    {
        public static string tableName = "weather_data";
        public override string TableName() => tableName;
        public override string InsertIdColumn() => "INSERT_ID";
        public override string Identifier(Dictionary<string, int> headerIndexes, List<string> line)
        {
            return line[headerIndexes["NUM_POSTE"]] + line[headerIndexes["AAAAMMJJHH"]];
        }
    }
}