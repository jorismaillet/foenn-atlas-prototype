namespace Assets.Scripts.Foenn.Atlas.Components.Holders
{
    using Assets.Scripts.Foenn.Atlas.Models.Geo;
    using Assets.Scripts.Foenn.Atlas.Models.Locations;
    using Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap.Go;
    using Assets.Scripts.Unity.Commons.Holders;
    using TMPro;

    public class GeoMeasureHolder : Holder<GeoMeasure>
    {
        public void SetPointLocation(PointLocationHolder holder)
        {
            holder.Initialize(element.point as PointLocation);
        }

        public void SetGeoPoint(GeoPointHolder holder)
        {
            holder.Initialize(element.point);
        }

        public void SetValue(TMP_Text text)
        {
            text.text = element.measure.value.ToString();
        }
    }
}
