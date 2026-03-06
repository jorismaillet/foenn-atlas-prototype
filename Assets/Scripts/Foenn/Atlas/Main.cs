using Assets.Scripts.Foenn.Atlas.Components.Holders;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL.Datasources;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Unity.Commons.Containers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            Env.SetDatabasePath(SqliteConnector.DATABASE_PATH);
            StartCoroutine(Init(MetadataDatasource.FilesToLoad()));
        }
        IEnumerator Init(List<string> filesToLoad)
        {
            if (filesToLoad.Any())
            {
                Application.runInBackground = true;
                yield return StartCoroutine(etlHandler.PrepareData(filesToLoad));
                Application.runInBackground = false;
            }
            map.Initialize(HourToLoad, department, key);
        }
    }
}