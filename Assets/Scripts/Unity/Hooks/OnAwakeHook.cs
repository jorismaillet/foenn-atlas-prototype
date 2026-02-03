using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine.Events;

namespace Assets.Scripts.Unity.Common.Behaviours
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
