using Assets.Scripts.Components.Commons.Holders;
using Assets.Scripts.Models.Geo;
using Assets.Scripts.Models.Locations;
using TMPro;

namespace Assets.Scripts.Components.Models
{
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
