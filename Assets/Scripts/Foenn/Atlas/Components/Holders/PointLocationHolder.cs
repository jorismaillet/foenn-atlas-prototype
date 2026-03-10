namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap.Go
{
    using Assets.Scripts.Foenn.Atlas.Components.Holders;
    using Assets.Scripts.Foenn.Atlas.Models.Locations;
    using Assets.Scripts.Unity.Commons.Holders;
    using TMPro;

    public class PointLocationHolder : Holder<PointLocation>
    {
        public void SetName(TMP_Text text)
        {
            text.text = element.name;
        }

        public void SetGeoPoint(GeoPointHolder holder)
        {
            holder.Initialize(element);
        }
    }
}
