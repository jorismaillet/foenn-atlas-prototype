using System.Linq;
using Assets.Scripts.Components.Commons.Containers;
using Assets.Scripts.Interface.Visualisations;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Layers
{
    public class DisplayGradient : MonoBehaviour
    {
        public PrefabsContainer container;
        public TMPro.TMP_Text title;
        public int nbValues = 7;

        public void Initialize(CustomGradient gradient) {
            var step = (gradient.maxValue - gradient.minValue) / (nbValues - 1);
            title.text = gradient.title;
            container.Initialize(Enumerable.Range(0, nbValues)
                .Select(i =>
                {
                    var value = gradient.minValue + step * i;
                    var color = gradient.GetColor(value, 1);
                    return new GradientValue(color, gradient.format, value);
                }).ToList());
        }
    }
}
