namespace Assets.Scripts.Unity.Commons.Containers
{
    using UnityEngine;

    public class PrefabsContainer : AbstractPrefabsContainer
    {
        public GameObject prefab;

        protected override GameObject ElementPrefab<Element>(Element element)
        {
            return prefab;
        }
    }
}
