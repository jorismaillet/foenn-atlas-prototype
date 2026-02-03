using UnityEngine;

namespace Assets.Scripts.Unity.Commons.Containers
{
    public class PrefabsContainer : AbstractPrefabsContainer
    {
        public GameObject prefab;

        protected override GameObject ElementPrefab<Element>(Element element)
        {
            return prefab;
        }
    }
}
