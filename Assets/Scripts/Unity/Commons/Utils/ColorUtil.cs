using UnityEngine;

namespace Assets.Scripts.Unity.Commons.Utils
{
    public class ColorUtil
    {
        public static void SetAlpha(CanvasGroup group, float alpha)
        {
            group.alpha = alpha;
        }

        public static Color Transparent(Color color)
        {
            return new Color(color.r, color.g, color.b, 0x00);
        }

        public static Color ColorFromHex(float r, float g, float b)
        {
            return new Color(r / 255F, g / 255F, b / 255F);
        }

        public static Color ColorFromHex(float r, float g, float b, float a)
        {
            return new Color(r / 255F, g / 255F, b / 255F, a / 255F);
        }
    }
}