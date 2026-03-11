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
            var geoMeasures = WeatherQueryService.FieldMeasuresPerPostForDay(day, month, year, department, key);
            heatmapContainer.SetMeasures(geoMeasures);
            pointmapContainer.Initialize(geoMeasures);
            pointLocationContainer.Initialize(new List<PointLocation>()
            {
                Seeds.brest, Seeds.tcPontLabbe, Seeds.tcQuimper, Seeds.maison
            });
            activityContainer.Initialize(new List<Activity>()
            {
                Seeds.piscine, Seeds.kayak, Seeds.plage, Seeds.velo, Seeds.jardin, Seeds.tennis, Seeds.ville, Seeds.randonee, Seeds.dinner
            });
            planningsContainer.Initialize(new List<Planning>()
            {
                Seeds.planningSportif, Seeds.ideesDeSorties
            });
        }
    }
}
