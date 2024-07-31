using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Activities
{
    public class TimeCondition
    {
        public int minHour, maxHour;

        public TimeCondition(int minHour, int maxHour)
        {
            this.minHour = minHour;
            this.maxHour = maxHour;
        }
    }
}