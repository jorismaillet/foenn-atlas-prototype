using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.OLAP.Datasets.WeatherHistory;
using Assets.Scripts.OLAP.Schema.Fields;
using UnityEngine;

namespace Assets.Scripts.Interface.Visualisations
{
    public class CustomPalette : ColorPicker
    {
        private static Dictionary<Field, Func<Dictionary<int, Color>>> Gradients = new Dictionary<Field, Func<Dictionary<int, Color>>>()
        {
            { WeatherHistoryDataset.Instance.coreFact.temperature, CreateTemperaturePalette }
        };

        public Dictionary<int, Color> palette;
        private int[] thresholds;

        public float minValue;

        public float maxValue;

        public CustomPalette(Field field) : base(field.displayName, field.format)
        {
            this.palette = Gradients[field].Invoke();
            this.thresholds = palette.Keys.ToArray();
        }

        public override Color GetColor(float value)
        {
            return palette.FirstOrDefault(kv => value <= kv.Key).Value;
        }

        private static Dictionary<int, Color> CreateTemperaturePalette()
        {
            return new Dictionary<int, Color>(){
                { -9, new Color(0.361f, 0.627f, 0.706f) }, // polar
                {  0, new Color(0.514f, 0.761f, 0.835f) }, // freezing
                {  7, new Color(0.404f, 0.584f, 0.408f) }, // very cold
                { 13, new Color(0.455f, 0.659f, 0.467f) }, // chilly
                { 18, new Color(0.553f, 0.749f, 0.443f) }, // cool
                { 24, new Color(0.914f, 0.800f, 0.467f) }, // comfortable
                { 29, new Color(0.780f, 0.459f, 0.376f) }, // hot
                { 35, new Color(0.690f, 0.357f, 0.353f) }, // very hot
                { 40, new Color(0.596f, 0.294f, 0.267f) }  // scorching
            };
        }
    }
}
