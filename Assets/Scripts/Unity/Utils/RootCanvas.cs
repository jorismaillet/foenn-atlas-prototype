namespace Assets.Scripts.Unity.Common.Utils
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine;

    [ExecuteInEditMode]
    public class RootCanvas : BaseBehaviour
    {
        public static Canvas instance;

        private void Awake()
        {
            instance = GetComponent<Canvas>();
        }
    }
}
