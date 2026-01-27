using Assets.Scripts.Unity.Utils.UIAnimations.Interpolations;
using UnityEngine;

namespace Assets.Scripts.Unity.Commons.UIAnimations {
    public class WaitAnimation : UIAnimation {
        public WaitAnimation(GameObject from, int durationMillis) : base(from, new LinearInterpolation(_ => { }, 0, 0), durationMillis) { }
    }
}