using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Components.Commons.Holders;
using UnityEngine;

namespace Assets.Scripts.Components.Commons.Containers
{
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
            El<Element>(gameObject).Initialize(element);
            return 0;
        }

        private IElementInitializer<Element> El<Element>(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out IElementInitializer<Element> res))
            {
                return res;
            }
            else
            {
                Debug.LogError($"{gameObject.name} is missing Initializer<{typeof(Element)}>", gameObject);
                return null;
            }
        }
    }
}
