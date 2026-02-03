using Assets.Scripts.Unity.Commons.Behaviours;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Scenes.Home.World.Views
{
    public class TextTimer : BaseBehaviour
    {
        private float remainingSec = 0;
        public Text text;

        public void SetTimer(int secs)
        {
            remainingSec = secs;
            SetText();
        }

        public void ClearTimer()
        {
            remainingSec = 0;
            text.text = string.Empty;
        }

        private void Update()
        {
            if (remainingSec <= 0)
            {
                return;
            }
            remainingSec = Math.Max(0, remainingSec - Time.deltaTime);
            SetText();
        }

        private void SetText()
        {
            text.text = ToTime((int)remainingSec);
        }

        private string ToTime(int secs)
        {
            TimeSpan time = TimeSpan.FromSeconds(secs);
            if (secs >= 3600)
            {
                return string.Format("{0}h {1}m", (int)time.TotalHours, time.Minutes);
            }
            else if (secs >= 60)
            {
                return string.Format("{0}m", (secs / 60) + 1);
            }
            else
            {
                return string.Format("{0}s", secs);
            }
        }
    }
}
