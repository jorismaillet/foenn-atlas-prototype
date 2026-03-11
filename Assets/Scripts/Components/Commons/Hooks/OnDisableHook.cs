namespace Assets.Scripts.Unity.Common.Hooks
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine.Events;

    public class OnDisableHook : BaseBehaviour
    {
        public UnityEvent onDisable;

        private void OnDisable()
        {
            onDisable.Invoke();
        }
    }
}
