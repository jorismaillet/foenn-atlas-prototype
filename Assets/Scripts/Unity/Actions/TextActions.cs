using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Common.Actions
{
    public class TextActions : BaseBehaviour
    {
        public void Clear(Text text)
        {
            text.text = string.Empty;
        }
    }
}
