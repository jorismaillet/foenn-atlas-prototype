namespace Assets.Scripts.Foenn.Atlas.Components.Holders
{
    using Assets.Scripts.Foenn.Atlas.Models.Activities;
    using Assets.Scripts.Unity.Commons.Holders;
    using TMPro;

    public class ActivityHolder : Holder<Activity>
    {
        public void SetName(TMP_Text text) {
            text.text = element.name;
        }
    }
}
