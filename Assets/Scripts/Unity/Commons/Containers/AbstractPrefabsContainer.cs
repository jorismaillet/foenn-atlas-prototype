namespace Assets.Scripts.Unity.Commons.Containers
{
    using Assets.Scripts.Unity.Commons.Holders;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public abstract class AbstractPrefabsContainer : GameObjectsContainer
    {
        protected abstract GameObject ElementPrefab<Element>(Element element);

        public void Initialize<Element>(IEnumerable<Element> elements)
        {
            Instantiate(Prefabs(elements)).Zip(elements, Initialize).ToList();
        }

        private IEnumerable<GameObject> Prefabs<Element>(IEnumerable<Element> elements)
        {
            foreach (var element in elements)
            {
                yield return ElementPrefab(element);
            }
            yield break;
        }

        private int Initialize<Element>(GameObject gameObject, Element element)
        {
            Holder<Element>(gameObject).Initialize(element);
            return 0;
        }

        private Holder<Element> Holder<Element>(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out Holder<Element> res))
            {
                return res;
            }
            else
            {
                Debug.LogError($"{gameObject.name} is missing Holder<{typeof(Element)}>", gameObject);
                return null;
            }
        }
    }
}
