using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Commons
{
    public class PrefabsContainer : MonoBehaviour
    {
        public GameObject prefab;

        public MutableList<GameObject> elements = new MutableList<GameObject>();

        public void Initialize<Element>(IEnumerable<Element> elements)
        {
            Instantiate(Prefabs(elements)).Zip(elements, Initialize).ToList();
        }

        private IEnumerable<GameObject> Instantiate(IEnumerable<GameObject> prefabs)
        {
            foreach (var previousElement in elements)
            {
                Destroy(previousElement);
            }
            elements.Clear();
            List<GameObject> newElements = new List<GameObject>();
            foreach (var prefab in prefabs)
            {
                var element = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform);
                yield return element;
                newElements.Add(element);
            }
            elements.AddRange(newElements);
            yield break;
        }

        private IEnumerable<GameObject> Prefabs<Element>(IEnumerable<Element> elements)
        {
            foreach (var element in elements)
            {
                yield return prefab;
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
