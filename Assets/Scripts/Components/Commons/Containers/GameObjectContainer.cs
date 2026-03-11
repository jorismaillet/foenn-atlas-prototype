namespace Assets.Scripts.Unity.Commons.Containers
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine;

    public class GameObjectContainer : BaseBehaviour
    {
        public bool clearPreviousElement = true;

        private GameObject previousElement;

        protected GameObject Instantiate(GameObject prefab)
        {
            if (clearPreviousElement && previousElement != null)
            {
                Destroy(previousElement);
            }
            previousElement = Instantiate(prefab, parent: transform);
            return previousElement;
        }
    }
}
