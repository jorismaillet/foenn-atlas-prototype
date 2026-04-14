using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Interface.Visualisations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Interface.Components.Layers
{
    public class DisplayGradientValue : MonoBehaviour, IElementInitializer<GradientValue>
    {
        public RawImage image;

        public TMP_Text valueText;

        public void Initialize(GradientValue gradientValue)
        {
            image.color = gradientValue.color;
            valueText.text = $"{gradientValue.value:0.##} {gradientValue.format}";
        }
    }
}
