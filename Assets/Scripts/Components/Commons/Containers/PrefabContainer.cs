namespace Assets.Scripts.Unity.Commons.Containers
{
    using UnityEngine;

    public class PrefabContainer : AbstractPrefabContainer
    {
        public GameObject prefab;

        protected override GameObject ElementPrefab<Element>(Element element)
        {
            return prefab;
        }
    }
}
