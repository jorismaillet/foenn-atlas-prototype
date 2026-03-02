using System;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap
{
    [Serializable]
    public class PointmapUiSettings
    {
        [Range(0f, 1f)] public float alpha = 1f;
        [Min(1f)] public float pointDiameterPx = 14f;
        public Color pointColor = Color.red;

        public void Validate()
        {
            if (alpha < 0f || alpha > 1f)
                throw new ArgumentException("Invalid alpha.");

            if (pointDiameterPx <= 0f)
                throw new ArgumentException("Invalid pointDiameterPx.");
        }
    }
}
