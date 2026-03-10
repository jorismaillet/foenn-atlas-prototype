namespace Assets.Scripts.Unity.Commons.UIAnimations
{
    using Assets.Scripts.Unity.Commons.UIAnimations.Interpolations;
    using UnityEngine;

    public class WaitAnimation : UIAnimation
    {
        public WaitAnimation(GameObject from, int durationMillis) : base(from, new LinearInterpolation(_ => { }, 0, 0), durationMillis)
        {
        }
    }
}
