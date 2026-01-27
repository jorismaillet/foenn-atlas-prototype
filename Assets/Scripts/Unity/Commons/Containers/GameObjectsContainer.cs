using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Unity.Commons.Containers {
    public class GameObjectsContainer : BaseBehaviour {
        public bool clearPreviousElements = true;
        private List<GameObject> previousElements = new List<GameObject>();
        public IEnumerable<GameObject> Instantiate(IEnumerable<GameObject> prefabs) {
            if (clearPreviousElements) {
                foreach(var previousElement in previousElements) {
                    Destroy(previousElement);
                }
                previousElements.Clear();
            }
            foreach(var prefab in prefabs) {
                var element = Instantiate(prefab, parent: transform);
                yield return element;
                previousElements.Add(element);
            }
            yield break;
        }
    }
}
