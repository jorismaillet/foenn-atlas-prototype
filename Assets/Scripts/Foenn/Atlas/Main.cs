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
            var sw = new Stopwatch();
            sw.Start();
            sw.Stop();
            UnityEngine.Debug.Log($"1 {sw.ElapsedMilliseconds}ms");
            sw.Start();
            sw.Stop();
            UnityEngine.Debug.Log($"2 {sw.ElapsedMilliseconds}ms");
            sw.Start();
            if (filesToLoad.Any())
            {
                Application.runInBackground = true;
                yield return StartCoroutine(etlHandler.PrepareData(filesToLoad));
                Application.runInBackground = false;
            }
            sw.Stop();
            UnityEngine.Debug.Log($"3 {sw.ElapsedMilliseconds}ms");
            sw.Start();
            map.Initialize(HourToLoad, department, key);
            sw.Stop();
            UnityEngine.Debug.Log($"4 {sw.ElapsedMilliseconds}ms");
            sw.Start();
            sw.Stop();
            UnityEngine.Debug.Log($"5 {sw.ElapsedMilliseconds}ms");
        }
    }
}