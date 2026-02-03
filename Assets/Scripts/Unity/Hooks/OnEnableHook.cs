using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine.Events;

namespace Assets.Scripts.Unity.Common.Behaviours
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
