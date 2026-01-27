using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Unity.Commons.Utils {
    public class ImageUtil {
        private static readonly string ImagesFolder = "Images/{0}";
        private static Dictionary<string, Sprite> spritesCache = new Dictionary<string, Sprite>();

        public static Sprite Get(string spritePath) {
            var key = string.Format(ImagesFolder, spritePath);
            if (!spritesCache.TryGetValue(key, out Sprite result)) {
                result = Resources.Load<Sprite>(key);
                spritesCache.Add(key, result);
            }
            return result;
        }

        public static void Set(Image image, string spritePath) {
            image.sprite = Get(spritePath);
        }
    }
}
