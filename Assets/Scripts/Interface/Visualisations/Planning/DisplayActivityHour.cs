using System.Linq;
using Assets.Scripts.Components.Commons.Holders;
using TMPro;

namespace Assets.Scripts.Interface.Visualisations.Planning
{
    public class DisplayActivityHour : Holder<ActivityHour>
    {
        public void SetHour(TMP_Text text)
        {
            text.text = element.hour.ToString();
        }

        public void SetActivities(TMP_Text text)
        {
            text.text = string.Join(", ", element.activities.Select(a => a.name));
        }
    }
}
