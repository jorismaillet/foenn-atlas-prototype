using Assets.Scripts.Components.Commons.Behaviours;
using UnityEngine.Events;

namespace Assets.Scripts.Components.Commons.Hooks
{
    public class OnAwakeHook : BaseBehaviour
    {
        public UnityEvent onAwake;

        private void Awake()
        {
            onAwake.Invoke();
        }
    }
}
