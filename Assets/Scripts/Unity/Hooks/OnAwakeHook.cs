namespace Assets.Scripts.Unity.Common.Behaviours
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine.Events;

    public class OnAwakeHook : BaseBehaviour
    {
        public UnityEvent onAwake;

        private void Awake()
        {
            onAwake.Invoke();
        }
    }
}
