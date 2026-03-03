using Assets.Scripts.Unity.Commons.Behaviours;
using Assets.Scripts.Unity.Commons.Mutables;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Unity.Commons.Containers
{
    public class GameObjectsContainer : BaseBehaviour
    {
        public bool clearPreviousElements = true;
        public MutableList<GameObject> elements = new MutableList<GameObject>();
        public IEnumerable<GameObject> Instantiate(IEnumerable<GameObject> prefabs)
        {
            if (clearPreviousElements)
            {
                foreach (var previousElement in elements)
                {
                    Destroy(previousElement);
                }
                elements.Clear();
            }
            List<GameObject> newElements = new List<GameObject>();
            foreach (var prefab in prefabs)
            {
                var element = Instantiate(prefab, parent: transform);
                yield return element;
                newElements.Add(element);
            }
            elements.AddRange(newElements);
            yield break;
        }
    }
}
