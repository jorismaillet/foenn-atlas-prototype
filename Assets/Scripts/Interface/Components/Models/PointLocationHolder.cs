using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Models.Locations;
using TMPro;

namespace Assets.Scripts.Components.Models
{
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
