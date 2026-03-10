namespace Assets.Scripts.Unity.Scenes.Home.World.Views
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class ClockTimer : BaseBehaviour
    {
        public int secs;

        private float remainingSec, maxSec;

        public Image image;

        public void ResetTimer()
        {
            remainingSec = secs;
            maxSec = secs;
        }

        private void Update()
        {
            if (remainingSec <= 0)
            {
                return;
            }
            remainingSec = Math.Max(0, remainingSec - Time.deltaTime);
            image.fillAmount = (maxSec - remainingSec) / maxSec;
        }
    }
}
