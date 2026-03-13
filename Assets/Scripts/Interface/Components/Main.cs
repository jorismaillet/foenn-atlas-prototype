using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Components;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Datasets.Metadata;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Assets.Scripts
{
    public class Main : MonoBehaviour
    {
        public ETLWorker etlHandler;

        public MapComponent map;

        void Start()
        {
            Env.SetDatabasePath(SqliteHelper.DATABASE_PATH);
            DatabaseHelper.CreateDb();
            using (var sqliteConnection = SqliteHelper.CreateConnection())
            {
                WeatherHistoryDataset.InitTables(sqliteConnection);
                var metadataTable = new MetadataTable(WeatherHistoryDataset.fact.name);
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
