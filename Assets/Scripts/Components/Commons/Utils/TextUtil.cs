namespace Assets.Scripts.Unity.Commons.Utils
{
    using UnityEngine.UI;

    public class TextUtil
    {
        public static void TrySet(Text property, string value)
        {
            if (property != null)
            {
                property.text = value;
            }
        }

        public static void Set(Text property, string value)
        {
            property.text = value;
        }
    }
}
