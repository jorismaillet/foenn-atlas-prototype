using Assets.Scripts.Unity.Commons.Behaviours;
using System;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Scenes.Fights.Views
{
    public class TimeCounter : BaseBehaviour
    {
        private DateTime startTime = DateTime.UtcNow;
        private Text duration;

        private void Awake()
        {
            duration = GetComponent<Text>();
            duration.text = "";
        }

        public void StartCounter()
        {
            startTime = DateTime.UtcNow;
        }

        private void Update()
        {
            double totalSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
            duration.text = totalSeconds.ToString();
        }
    }
}
