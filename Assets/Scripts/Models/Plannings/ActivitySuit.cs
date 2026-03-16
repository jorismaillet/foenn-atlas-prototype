using Assets.Scripts.Models.Activities;

namespace Assets.Scripts.Models.Plannings
{
    public class ActivitySuit
    {
        public readonly Activity activity;

        public readonly int suitScore;

        public ActivitySuit(Activity activity, int suitScore)
        {
            this.activity = activity;
            this.suitScore = suitScore;
        }
    }
}
