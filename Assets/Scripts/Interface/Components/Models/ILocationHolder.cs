using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Models.Locations;
using TMPro;

namespace Assets.Scripts.Components.Models
{
    public class ILocationHolder : Holder<ILocation>
    {
        public void SetName(TMP_Text text)
        {
            text.text = element.Name;
        }
    }
}
