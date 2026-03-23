using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
            for (int i = 0; i < nbValues; i++)
            {
                var value = gradient.minValue + step * i;
                var color = gradient.GetColor(value, 1);
                var go = (GameObject)PrefabUtility.InstantiatePrefab(container.prefab, container.transform);
                go.GetComponent<GradientValue>().Initialize(color, gradient.format, value);
            }
        }
    }
}
