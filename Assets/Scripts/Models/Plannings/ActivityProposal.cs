using System;
using Assets.Scripts.Models.Activities;

namespace Assets.Scripts.Models.Plannings
{
    public class ActivityProposal
    {
        public Activity activity;

        public DateTime startTime;

        public int duration;

        public int recommendationScore;

        public ActivityProposal(Activity activity, DateTime startTime, int duration, int recommendationScore)
        {
            this.activity = activity;
            this.startTime = startTime;
            this.duration = duration;
            this.recommendationScore = recommendationScore;
        }
    }
}
