using Assets.Scripts.Components.Commons.Behaviours;
using UnityEngine;

namespace Assets.Scripts.Components.Commons.Containers
{
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
