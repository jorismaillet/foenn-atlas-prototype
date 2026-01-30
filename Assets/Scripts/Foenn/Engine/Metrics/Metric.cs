using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Engine.Metrics
{
    public class Metric : AbstractMetric
    {
        public float value;

        public override float Value()
        {
            return value;
        }
    }
}