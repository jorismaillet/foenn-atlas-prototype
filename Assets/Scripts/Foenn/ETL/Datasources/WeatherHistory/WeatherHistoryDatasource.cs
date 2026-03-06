using Assets.Scripts.Foenn.ETL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using static Codice.CM.WorkspaceServer.DataStore.WkTree.WriteWorkspaceTree;

namespace Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory
{
    public class WeatherHistoryDatasource : Datasource
    {
        public static string tableName = "weather_data";
        public override string TableName() => tableName;

        private string[] extraColumns = new string[2];
        private string dpt;

        public WeatherHistoryDatasource(string dpt) : base()
        {
            this.dpt = dpt;
        }

        public override void PrepareSchema(SchemaDefinition schema)
        {
            schema.columns.Add(new PrimaryKey("ID", DbType.Int64, true));
            schema.columns.Add(new Datafield("DPT", DbType.String));

            schema.indexes.Add(new IndexDefinition(true, "ID"));
            schema.indexes.Add(new IndexDefinition(false, "DPT"));
            schema.indexes.Add(new IndexDefinition(false, "AAAAMMJJHH"));
            schema.indexes.Add(new IndexDefinition(false, "NUM_POSTE"));
            schema.indexes.Add(new IndexDefinition(false, "NOM_USUEL"));
            schema.indexes.Add(new IndexDefinition(false, "LAT", "LON"));
            schema.indexes.Add(new IndexDefinition(true, "NUM_POSTE", "AAAAMMJJHH"));
        }

        public override string[] GetExtraColumns(string[] columns)
        {
            extraColumns[1] = dpt;
            return extraColumns;
        }
    }
}