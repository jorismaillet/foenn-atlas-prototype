namespace Assets.Scripts.Unity.Commons.Containers
{
    using UnityEngine;

    public abstract class CustomPrefabContainer<Element> : AbstractPrefabContainer where Element : class
    {
        protected override GameObject ElementPrefab<BaseElement>(BaseElement element)
        {
            return Prefab(element as Element);
        }

        public abstract GameObject Prefab(Element element);
    }
}
