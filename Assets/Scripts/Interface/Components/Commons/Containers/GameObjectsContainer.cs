using System.Collections.Generic;
using Assets.Scripts.Components.Commons.Behaviours;
using Assets.Scripts.Components.Commons.Mutables;
using UnityEngine;

namespace Assets.Scripts.Components.Commons.Containers
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
                var element = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform);
                yield return element;
                newElements.Add(element);
            }
            elements.AddRange(newElements);
            yield break;
        }
    }
}
