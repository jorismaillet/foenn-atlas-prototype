using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Interface.Components.Layers
{
    public class GradientValue : MonoBehaviour
    {
        public RawImage image;
        public TMP_Text valueText;

        internal void Initialize(Color color, string format, float value)
        {
            image.color = color;
            valueText.text = $"{value:0.##} {format}";
        }
    }
}
