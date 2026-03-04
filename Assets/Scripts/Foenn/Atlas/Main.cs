using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap;
using Assets.Scripts.Foenn.Engine.Connectors;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Unity.Commons.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas
{
    public class Main : MonoBehaviour
    {
        public ETLHandler etlHandler;
        public PrefabsContainer pointmapContainer;
        public HeatmapWorldOverlay heatmapContainer;
        public string HourToLoad = "2023080110";

        void Start()
        {
            StartCoroutine(Init());
        }
        IEnumerator Init()
        {
            Env.SetDatabasePath(SqliteConnector.DATABASE_PATH);
            yield return StartCoroutine(etlHandler.PrepareData());
            List<GeoMeasure> measures = PreconfiguredRequest.WeatherHistoryMeasures(HourToLoad, WeatherHistoryMetricKey.T);
            heatmapContainer.SetMeasures(measures);
            pointmapContainer.Initialize(measures);
        }
    }
}