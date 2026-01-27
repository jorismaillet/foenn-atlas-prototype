using System;

namespace Assets.Scripts.Unity.Commons.UIAnimations.Interpolations {
    public class IncreasingInterpolation : Interpolation {
        public IncreasingInterpolation(Action<float> action, float startValue, float endValue) : base(action, startValue, endValue) {
        }

        public override float Interpolate(float coef) {
            float startCoef = (float)Math.Cos(coef * (Math.PI / 2.0D));
            float endCoef = 1.0F - startCoef;
            return startValue * startCoef + endCoef * endValue;
        }
    }
}