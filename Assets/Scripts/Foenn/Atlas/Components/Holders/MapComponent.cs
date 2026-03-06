using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Commons.Containers;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Components.Holders
{
    public class MapComponent : MonoBehaviour
    {
        public PrefabsContainer pointmapContainer;
        public HeatmapWorldOverlay heatmapContainer;

        public void Initialize(string hourToLoad, string department, WeatherHistoryMetricKey key)
        {
            var sw = new Stopwatch();
            sw.Start();
            var geoMeasures = PreconfiguredRequest.WeatherHistoryMeasures(hourToLoad, department, WeatherHistoryMetricKey.T);
            sw.Stop();
            UnityEngine.Debug.Log($"Models built in {sw.ElapsedMilliseconds} ms");
            var sw2 = new Stopwatch();
            sw2.Start();
            heatmapContainer.SetMeasures(geoMeasures);
            pointmapContainer.Initialize(geoMeasures);
            sw2.Stop();
            UnityEngine.Debug.Log($"MapComponent initialized in {sw2.ElapsedMilliseconds} ms");
        }
    }
}
