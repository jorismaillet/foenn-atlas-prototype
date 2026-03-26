using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Models.Plannings;
using TMPro;

namespace Assets.Scripts.Components.Models
{
    public class PlanningHolder : Holder<Planning>
    {
        public void SetTitle(TMP_Text text)
        {
            text.text = element.title;
        }

        public void SetPlannedActivities(PrefabsContainer container)
        {
            container.Initialize(element.plannedActivities);
        }
    }
}
