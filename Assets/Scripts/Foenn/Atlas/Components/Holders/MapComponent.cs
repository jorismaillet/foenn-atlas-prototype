using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Foenn.Atlas.Models.Plannings;
using Assets.Scripts.Foenn.Engine.Execution;
using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Commons.Containers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Components.Holders
{
    public class MapComponent : MonoBehaviour
    {
        public PrefabsContainer pointLocationContainer;
        public PrefabsContainer pointmapContainer;
        public PrefabsContainer activityContainer;
        public PrefabsContainer planningsContainer;
        public HeatmapWorldOverlay heatmapContainer;

        public void Initialize(string hourToLoad, string department, WeatherHistoryMetricKey key)
        {
            var geoMeasures = PreconfiguredRequest.WeatherHistoryMeasures(hourToLoad, department, WeatherHistoryMetricKey.T);
            heatmapContainer.SetMeasures(geoMeasures);
            pointmapContainer.Initialize(geoMeasures);
            pointLocationContainer.Initialize(new List<PointLocation>()
            {
                Seed.brest, Seed.tcPontLabbe, Seed.tcQuimper, Seed.maison
            });
            activityContainer.Initialize(new List<Activity>()
            {
                Seed.piscine, Seed.kayak, Seed.plage, Seed.velo, Seed.jardin, Seed.tennis, Seed.ville, Seed.randonee, Seed.dinner
            });
            planningsContainer.Initialize(new List<Planning>()
            {
                Seed.planningSportif, Seed.ideesDeSorties
            });
        }
    }
}
