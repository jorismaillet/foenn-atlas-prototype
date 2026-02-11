using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;
using static Codice.CM.WorkspaceServer.DataStore.WkTree.WriteWorkspaceTree;

namespace Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory
{
    public class WeatherHistoryDatasource : Datasource
    {
        public static string tableName = "weather_data";
        public override string TableName() => tableName;
        public override string InsertIdColumn() => "INSERT_ID";
        public override string Identifier(Dictionary<string, int> headerIndexes, string[] line)
        {
            return line[headerIndexes["NUM_POSTE"]] + line[headerIndexes["AAAAMMJJHH"]];
        }

        private string[] extraColumns = new string[1];
        private int numPostIndex;
        private int dateIndex;

        public WeatherHistoryDatasource() : base(1)
        {
        }

        public override void PrepareTranformer(SchemaDefinition schema)
        {
            numPostIndex = schema.headersIndexes["NUM_POSTE"];
            dateIndex = schema.headersIndexes["AAAAMMJJHH"];
        }

        public override string[] GetExtraColumns(string[] columns)
        {
            extraColumns[0] = columns[numPostIndex] + columns[dateIndex];
            return extraColumns;
        }
    }
}