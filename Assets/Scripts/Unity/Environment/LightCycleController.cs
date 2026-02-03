using Assets.Scripts.Common.Utils;
using Assets.Scripts.Unity.Commons.Behaviours;
using System;
using UnityEngine;

namespace Assets.Scripts.Unity.Environment
{
    public class LightCycleController : BaseBehaviour
    {
        public bool pauseCycle = false, pauseRotation = false, pauseIntensity = false;

        public float riseStart, riseEnd, fallStart, fallEnd, cycleOffset, minIntensity, maxIntensity;

        public float cycleInSeconds = 30F;
        [Range(0, 1)]
        public float cycle = 0;

        private new Light light;
        private DateTime startTime = DateTime.UtcNow;

        private void Start()
        {
            light = GetComponent<Light>();
        }

        void Update()
        {
            UpdateRotation();
            UpdateIntensity();
            if (!pauseCycle)
            {
                cycle = Convert.ToSingle((DateTime.UtcNow.Subtract(startTime).TotalSeconds / cycleInSeconds) + cycleOffset) % 1.0F;
            }
        }

        public int yRotation = 170, zRotation = 0;

        private void UpdateRotation()
        {
            if (!pauseRotation)
            {
                light.transform.localRotation = Quaternion.Euler((cycle * 180f), yRotation, zRotation);
            }
        }

        private void UpdateIntensity()
        {
            float intensity;
            if (cycle < riseStart || cycle > fallEnd)
            {
                intensity = minIntensity;
            }
            else if (cycle > riseEnd && cycle < fallStart)
            {
                intensity = maxIntensity;
            }
            else if (cycle >= riseStart && cycle <= riseEnd)
            {
                intensity = MathUtil.Lerp(riseStart, riseEnd, cycle, minIntensity, maxIntensity);
            }
            else
            {
                intensity = MathUtil.Lerp(fallStart, fallEnd, cycle, maxIntensity, minIntensity);
            }
            light.intensity = intensity;
        }
    }
}
