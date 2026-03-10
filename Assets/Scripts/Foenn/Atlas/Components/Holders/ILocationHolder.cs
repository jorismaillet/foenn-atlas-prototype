namespace Assets.Scripts.Foenn.Atlas.Components.Holders
{
    using Assets.Scripts.Foenn.Atlas.Models.Locations;
    using Assets.Scripts.Unity.Commons.Holders;
    using TMPro;

    public class ILocationHolder : Holder<ILocation>
    {
        public void SetName(TMP_Text text)
        {
            text.text = element.Name;
        }
    }
}
