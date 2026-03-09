using Assets.Scripts.Foenn.Atlas.Components.Holders;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.ETL.Datasets;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas
{
    public class Main : MonoBehaviour
    {
        public ETLHandler etlHandler;
        public string HourToLoad = "2023080110";
        [SerializeField] public string department = "29";
        public WeatherHistoryMetricKey key;
        public MapComponent map;

        void Start()
        {
            Env.SetDatabasePath(SqliteHelper.DATABASE_PATH);
            using (var sqliteConnection = SqliteHelper.CreateConnection()) {
                var weatherHistoryDataset = new WeatherHistoryDataset();
                weatherHistoryDataset.InitTables(sqliteConnection);
                var metadataTable = new MetadataTable(weatherHistoryDataset.Name);
                metadataTable.InitTable(sqliteConnection);
                StartCoroutine(Init(sqliteConnection, metadataTable.FilesToLoad(sqliteConnection), metadataTable, weatherHistoryDataset));
            }
        }
        IEnumerator Init(SqliteConnection connection, List<string> filesToLoad, MetadataTable metadata, WeatherHistoryDataset dataset)
        {
            if (filesToLoad.Any())
            {
                Application.runInBackground = true;
                yield return StartCoroutine(etlHandler.PrepareData(connection, filesToLoad, metadata, dataset));
                Application.runInBackground = false;
            }
            map.Initialize(HourToLoad, department, key);
        }
    }
}