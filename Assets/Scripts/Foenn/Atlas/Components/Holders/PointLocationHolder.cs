using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Models.Locations;
using Assets.Scripts.Unity.Commons.Holders;
using TMPro;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap.Go
{
    public class PointLocationHolder : Holder<PointLocation>
    {
        public void SetName(TMP_Text text)
        {
            text.text = element.name;
        }
        public void SetGeoPoint(Holder<GeoPoint> holder)
        {
            holder.Initialize(element);
        } 
    }
}
