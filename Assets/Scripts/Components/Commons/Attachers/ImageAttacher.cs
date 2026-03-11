namespace Assets.Scripts.Unity.Commons.Attachers
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public abstract class ImageAttacher<Element> : Attacher<Element> where Element : class
    {
        private static readonly string ImagesFolder = "Images/{0}";

        private static Dictionary<string, Sprite> backgroundSpriteCache = new Dictionary<string, Sprite>();

        private static Sprite Get(string key)
        {
            if (!backgroundSpriteCache.TryGetValue(key, out Sprite result))
            {
                result = Resources.Load<Sprite>(key);
                backgroundSpriteCache.Add(key, result);
            }
            return result;
        }

        public abstract string SpritePath(Element element);

        public override void Initialize(Element element)
        {
            GetComponent<Image>().sprite = Get(string.Format(ImagesFolder, SpritePath(element)));
        }
    }
}
