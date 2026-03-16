using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Components;
using Assets.Scripts.Database;
using Assets.Scripts.OLAP.Datasets;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using UnityEngine;

namespace Assets.Scripts
{
    public class Main : MonoBehaviour
    {
        public ETLWorker etlHandler;

        public MapComponent map;

        void Start()
        {
            Env.DatabasePath = SqliteHelper.DATABASE_PATH;
            SqliteHelper.CreateDb();

            var dataset = WeatherHistoryDataset.Instance;
            List<string> filesToLoad;

            using (var sqliteConnection = SqliteHelper.CreateConnection())
            {
                WeatherHistoryDataset.Instance.InitTables(sqliteConnection);
                SqliteHelper.CreateTable(sqliteConnection, dataset.MetadataTable);
                filesToLoad = dataset.MetadataTable.FilesToLoad(sqliteConnection);
            }

            StartCoroutine(Init(dataset, filesToLoad));
        }

        IEnumerator Init(Dataset dataset, List<string> filesToLoad)
        {
            if (filesToLoad.Any())
            {
                Application.runInBackground = true;
                Application.targetFrameRate = 1;
                yield return StartCoroutine(etlHandler.PrepareData(dataset, filesToLoad));
                Application.runInBackground = false;
                Application.targetFrameRate = 60;
            }
            map.Initialize();
        }
    }
}
