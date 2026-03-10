namespace Assets.Scripts.Foenn.Atlas.Components.Holders
{
    using Assets.Scripts.Foenn.Atlas.Models.Plannings;
    using Assets.Scripts.Unity.Commons.Containers;
    using Assets.Scripts.Unity.Commons.Holders;
    using TMPro;

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
