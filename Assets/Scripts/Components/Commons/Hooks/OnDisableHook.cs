using Assets.Scripts.Components.Commons.Behaviours;
using UnityEngine.Events;

namespace Assets.Scripts.Components.Commons.Hooks
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
