using Assets.Scripts.Foenn.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations
{
    public class HourPoint
    {
        public int x, y;
        public Color color;

        private static Color defaultColor = ColorUtils.Get(28, 28, 27);

        public HourPoint(DateTime time, Color color)
        {
            x = time.DayOfYear - 1;
            y = time.Hour;
            this.color = color;
        }
    }
}