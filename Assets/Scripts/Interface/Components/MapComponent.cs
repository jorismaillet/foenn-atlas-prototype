using System.Collections.Generic;
using Assets.Scripts.Components.Commons.Containers;
using Assets.Scripts.Components.Visualisations.Heatmap;
using Assets.Scripts.Models.Activities;
using Assets.Scripts.Models.Locations;
using Assets.Scripts.Models.Plannings;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.Components
{
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

        public string key;

        public void Initialize()
        {
            var geoMeasures = WeatherQueryService.DayObservationsForPost(day, month, year, department, key);
            heatmapContainer.SetMeasures(geoMeasures);
            pointmapContainer.Initialize(geoMeasures);
            pointLocationContainer.Initialize(new List<PointLocation>()
            {
                Seeds.brestCenter, Seeds.pontLabbeTennis, Seeds.quimperTennis, Seeds.plomelinCenter
            });
            activityContainer.Initialize(new List<Activity>()
            {
                Seeds.swimming, Seeds.kayak, Seeds.beach, Seeds.biking, Seeds.gardening, Seeds.tennis, Seeds.cityPromenade, Seeds.hiking, Seeds.dinner
            });
            planningsContainer.Initialize(new List<Planning>()
            {
                Seeds.sportsPlanning, Seeds.outsideActivities
            });
        }
    }
}
