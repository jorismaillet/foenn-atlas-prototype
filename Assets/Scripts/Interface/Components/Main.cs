using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Components;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Datasets.Metadata;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Datasets.WeatherHistory.coreFacts;
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

            List<string> filesToLoad;
            var metadataTable = new MetadataTable(WeatherHistoryDataset.coreFact.name);

            using (var sqliteConnection = SqliteHelper.CreateConnection())
            {
                WeatherHistoryDataset.InitTables(sqliteConnection);
                SqliteHelper.CreateTable(sqliteConnection, metadataTable);
                filesToLoad = metadataTable.FilesToLoad(sqliteConnection);
            }

            StartCoroutine(Init(filesToLoad, metadataTable));
        }

        IEnumerator Init(List<string> filesToLoad, MetadataTable metadata)
        {
            if (filesToLoad.Any())
            {
                Application.runInBackground = true;
                Application.targetFrameRate = 1;
                yield return StartCoroutine(etlHandler.PrepareData(filesToLoad, metadata));
                Application.runInBackground = false;
                Application.targetFrameRate = 60;
            }
            map.Initialize();
        }
    }
}
