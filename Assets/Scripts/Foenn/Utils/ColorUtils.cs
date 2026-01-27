using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Utils
{
    public class ColorUtils
    {
        public static Color Get(int r, int g, int b)
        {
            return new Color(r/255.0F, g/255.0F, b/255.0F);
        }
    }
}