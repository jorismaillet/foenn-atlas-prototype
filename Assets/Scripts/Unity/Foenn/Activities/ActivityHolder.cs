using Assets.Scripts.Foenn.Atlas.Models.Activities;
using Assets.Scripts.Unity.Commons.Holders;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Foenn.Activities
{
    public class ActivityHolder : Holder<Activity>
    {
        public void SetName(Text text)
        {
            text.text = element.name;
        }
    }
}
