using Assets.Scripts.Components.Commons.Containers;
using Assets.Scripts.Components.Visualisations.Heatmap;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Scenarios
{
    public class ActivityStatistics : MonoBehaviour
    {
        public PrefabsContainer pointmapContainer;
        public HeatmapWorldOverlay heatmapContainer;

        public TMPro.TMP_Dropdown yearDropdown;

        public void OnCaseSelected()
        {

        }
    }
}
