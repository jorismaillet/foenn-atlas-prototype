using Assets.Scripts.OLAP.Schema.Fields;
using UnityEngine;

namespace Assets.Scripts.Interface.Visualisations
{
    public abstract class ColorPicker
    {
        public abstract Color GetColor(float value);

        public string title;

        public string format;

        public ColorPicker(string title, string format)
        {
            this.title = title;
            this.format = format;
        }
    }
}
