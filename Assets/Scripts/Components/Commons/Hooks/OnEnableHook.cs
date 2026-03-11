using Assets.Scripts.Components.Commons.Behaviours;
using UnityEngine.Events;

namespace Assets.Scripts.Components.Commons.Hooks
{
    public class OnEnableHook : BaseBehaviour
    {
        public UnityEvent onEnable;

        private void OnEnable()
        {
            onEnable.Invoke();
        }
    }
}
