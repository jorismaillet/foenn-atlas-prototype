namespace Assets.Scripts.Foenn.Atlas
{
    using Assets.Scripts.Foenn.Atlas.Components.Holders;
    using Assets.Scripts.Foenn.Core.Database;
    using Assets.Scripts.Foenn.OLAP.Datasets.WeatherHistory;
    using Mono.Data.Sqlite;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class Main : MonoBehaviour
    {
        public ETLHandler etlHandler;

        public MapComponent map;

        void Start()
        {
            Env.SetDatabasePath(SqliteHelper.DATABASE_PATH);
            using (var sqliteConnection = SqliteHelper.CreateConnection())
            {
                WeatherHistoryDataset.InitTables(sqliteConnection);
                var metadataTable = new MetadataTable(WeatherHistoryDataset.fact.Name);
                metadataTable.InitTable(sqliteConnection);
                StartCoroutine(Init(sqliteConnection, metadataTable.FilesToLoad(sqliteConnection), metadataTable));
            }
        }

        IEnumerator Init(SqliteConnection connection, List<string> filesToLoad, MetadataTable metadata)
        {
            if (filesToLoad.Any())
            {
                Application.runInBackground = true;
                yield return StartCoroutine(etlHandler.PrepareData(connection, filesToLoad, metadata));
                Application.runInBackground = false;
            }
            map.Initialize();
        }
    }
}
