using System;
using Assets.Scripts.Models.Activities;

namespace Assets.Scripts.Models.Plannings
{
    public class ActivityProposal
    {
        public readonly Activity activity;

        public readonly DateTime startTime;

        public readonly int duration;

        public readonly int recommendationScore;

        public ActivityProposal(Activity activity, DateTime startTime, int duration, int recommendationScore)
        {
            this.activity = activity;
            this.startTime = startTime;
            this.duration = duration;
            this.recommendationScore = recommendationScore;
        }
    }
}
