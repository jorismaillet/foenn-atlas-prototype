namespace Assets.Scripts.Unity.Commons.Containers
{
    using Assets.Scripts.Unity.Commons.Holders;
    using UnityEngine;

    public abstract class AbstractPrefabContainer : GameObjectContainer
    {
        protected abstract GameObject ElementPrefab<Element>(Element element);

        public void Initialize<Element>(Element element)
        {
            Initialize(Instantiate(ElementPrefab(element)), element);
        }

        private void Initialize<Element>(GameObject gameObject, Element element)
        {
            Holder<Element>(gameObject).Initialize(element);
        }

        private Holder<Element> Holder<Element>(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out Holder<Element> res))
            {
                return res;
            }
            else
            {
                Debug.LogError($"{gameObject.name} is missing Holder<{typeof(Element)}>", gameObject);
                return null;
            }
        }
    }
}
