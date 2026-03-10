namespace Assets.Scripts.Unity.Common.Views
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using UnityEngine;

    public class FollowMouse : BaseBehaviour
    {
        private void OnEnable()
        {
            transform.position = Input.mousePosition;
        }

        private void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}
