namespace Assets.Scripts.Unity.Commons.Containers
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class CapacityIntContainer : GameObjectsContainer
    {
        public GameObject inCapacityPrefab, outCapacityPrefab;

        public void Initialize(int nbItems, int maxItems)
        {
            var inCapacity = Prefabs(nbItems, inCapacityPrefab).ToList();
            var outCapacity = Prefabs(maxItems - nbItems, outCapacityPrefab).ToList();
            Instantiate(inCapacity.Concat(outCapacity)).ToList();
        }

        private IEnumerable<GameObject> Prefabs(int nbItems, GameObject prefab)
        {
            for (int i = 0; i < nbItems; i++)
            {
                yield return prefab;
            }
            yield break;
        }
    }
}
