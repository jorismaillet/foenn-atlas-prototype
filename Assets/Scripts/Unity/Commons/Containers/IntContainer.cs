using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Unity.Commons.Containers
{
    public class IntContainer : GameObjectsContainer
    {
        public GameObject prefab;
        public void Initialize(int nbItems)
        {
            Instantiate(Prefabs(nbItems)).ToList();
        }
        private IEnumerable<GameObject> Prefabs(int nbItems)
        {
            for (int i = 0; i < nbItems; i++)
            {
                yield return prefab;
            }
            yield break;
        }
    }
}
