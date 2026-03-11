namespace Assets.Scripts.Unity.Common.Behaviours
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine.Events;

    public class OnEnableHook : BaseBehaviour
    {
        public UnityEvent onEnable;

        private void OnEnable()
        {
            onEnable.Invoke();
        }
    }
}
