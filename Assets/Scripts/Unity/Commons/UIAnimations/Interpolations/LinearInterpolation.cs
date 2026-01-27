using System;

namespace Assets.Scripts.Unity.Commons.UIAnimations.Interpolations {
    public class LinearInterpolation : Interpolation {
        public LinearInterpolation() : base(null, 0, 0) { }

        public LinearInterpolation(Action<float> action, float startValue, float endValue) : base(action, startValue, endValue) {
        }

        public override float Interpolate(float coef) {
            return startValue * (1.0F - coef) + coef * endValue;
        }
    }
}