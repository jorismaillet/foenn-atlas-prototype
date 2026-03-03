using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Unity.Commons.Holders;
using TMPro;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap.Go
{
    public class GeoMeasureHolder : Holder<GeoMeasure>
    {
        public void SetValue(TMP_Text text)
        {
            text.text = element.value.ToString();
        }
    }
}
