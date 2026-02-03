using UnityEngine;

namespace Assets.Scripts.Unity.Commons.Containers
{
    public class PrefabContainer : AbstractPrefabContainer
    {
        public GameObject prefab;

        protected override GameObject ElementPrefab<Element>(Element element)
        {
            return prefab;
        }
    }
}
