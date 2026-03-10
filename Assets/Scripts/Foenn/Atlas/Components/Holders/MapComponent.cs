namespace Assets.Scripts.Foenn.Atlas.Components.Holders
{
    using Assets.Scripts.Foenn.Atlas.Models.Activities;
    using Assets.Scripts.Foenn.Atlas.Models.Locations;
    using Assets.Scripts.Foenn.Atlas.Models.Plannings;
    using Assets.Scripts.Foenn.Engine.Execution;
    using Assets.Scripts.Foenn.ETL.Datasources.WeatherHistory;
    using Assets.Scripts.Unity.Commons.Containers;
    using System.Collections.Generic;
    using UnityEngine;

    public class MapComponent : MonoBehaviour
    {
        public PrefabsContainer pointLocationContainer;

        public PrefabsContainer pointmapContainer;

        public PrefabsContainer activityContainer;

        public PrefabsContainer planningsContainer;

        public HeatmapWorldOverlay heatmapContainer;

        [SerializeField]
        public int day = 01, month = 08, year = 2023;

        [SerializeField]
        public string department = "29";

        public WeatherHistoryMetricKey key;

        public void Initialize()
        {
            var geoMeasures = PreconfiguredRequest.FieldMeasuresPerPostForDay(day, month, year, department, key);
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
