using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine;

namespace Assets.Scripts.Unity.Common.Views
{
    [ExecuteInEditMode]
    public class FollowTransform : BaseBehaviour
    {
        public Transform target { get; private set; }
        public Vector3 offset;

        private void Update()
        {
            if (target != null)
            {
                transform.position = target.position + offset;
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            SetPosition();
        }

        private void SetPosition()
        {
            transform.position = target.position + offset;
        }
    }
}
