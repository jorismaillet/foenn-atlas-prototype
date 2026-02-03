using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Foenn.Utils
{
    public class ColorUtils
    {

        public UnityEngine.Color defaultActivityColor;

        public static UnityEngine.Color
            VertJardin = ColorUtils.Get(58, 251, 3),
            bleuCiel = ColorUtils.Get(3, 251, 248),
            bleuMer = ColorUtils.Get(15, 75, 227),
            sable = ColorUtils.Get(241, 255, 0),
            asphalteClaire = ColorUtils.Get(234, 234, 234),
            balleTennis = ColorUtils.Get(218, 255, 0),
            murs = ColorUtils.Get(144, 144, 144),
            vertForet = ColorUtils.Get(29, 122, 29),
            crepuscule = ColorUtils.Get(255, 183, 0),
            rouge = ColorUtils.Get(255, 0, 0);

        public static Color Get(int r, int g, int b)
        {
            return new Color(r/255.0F, g/255.0F, b/255.0F);
        }
    }
}