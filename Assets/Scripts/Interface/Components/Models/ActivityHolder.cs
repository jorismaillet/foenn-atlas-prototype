using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Models.Activities;
using TMPro;

namespace Assets.Scripts.Components.Models
{
    public class ActivityHolder : Holder<Activity>
    {
        public void SetName(TMP_Text text)
        {
            text.text = element.name;
        }
    }
}
