namespace Assets.Scripts.Unity.Common.Actions
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine.UI;

    public class TextActions : BaseBehaviour
    {
        public void Clear(Text text)
        {
            text.text = string.Empty;
        }
    }
}
