namespace Assets.Scripts.Foenn.Atlas.Models.Plannings
{
    using Assets.Scripts.Foenn.Atlas.Models.Activities;
    using System;

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
