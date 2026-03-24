using UnityEngine;

namespace Assets.Scripts.Interface.Visualisations
{
    public class GradientValue
    {
        public Color color;
        public string format;
        public float value;

        public GradientValue(Color color, string format, float value)
        {
            this.color = color;
            this.format = format;
            this.value = value;
        }
    }
}
