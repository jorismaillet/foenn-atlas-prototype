using Assets.Scripts.Foenn.Atlas.Models.Condition;

namespace Assets.Scripts.Foenn.Atlas.Models.Activities
{
    public class Activity
    {
        public string name;
        public AllCondition conditions;

        public Activity(string name, params ICondition[] conditions)
        {
            this.name = name;
            this.conditions = new AllCondition(conditions);
        }
    }
}
