namespace Assets.Scripts.Unity.Common.Behaviours
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public class DoAfterMillis : BaseBehaviour
    {
        public UnityEvent action;

        public int waitMillis;

        private void Awake()
        {
            StartCoroutine(AsyncDo());
        }

        private IEnumerator AsyncDo()
        {
            yield return new WaitForSeconds(waitMillis / 1000.0F);
            action.Invoke();
        }
    }
}
