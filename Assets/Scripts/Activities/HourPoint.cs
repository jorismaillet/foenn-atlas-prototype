using Assets.Resources.Activities;
using System;
using UnityEngine;

namespace Assets.Scripts.Activities
{
    public class HourPoint
    {
        public int x, y;
        public Color color;

        private static Color defaultColor = ColorHelper.Get(28, 28, 27);

        public HourPoint(DateTime time, Activity activity)
        {
            x = time.DayOfYear - 1;
            y = time.Hour;
            if(activity == null)
            {
                color = defaultColor;
            }
            else
            {
                this.color = activity.color;
            }
        }
    }
}