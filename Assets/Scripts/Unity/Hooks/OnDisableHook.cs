using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine.Events;

namespace Assets.Scripts.Unity.Common.Hooks
{
    public class OnDisableHook : BaseBehaviour
    {
        public UnityEvent onDisable;

        private void OnDisable()
        {
            onDisable.Invoke();
        }
    }
}
