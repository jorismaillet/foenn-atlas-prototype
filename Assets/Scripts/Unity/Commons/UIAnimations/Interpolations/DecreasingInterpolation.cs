namespace Assets.Scripts.Unity.Commons.UIAnimations.Interpolations
{
    using System;

    public class DecreasingInterpolation : Interpolation
    {
        public DecreasingInterpolation() : base(null, 0, 0)
        {
        }

        public DecreasingInterpolation(Action<float> action, float startValue, float endValue) : base(action, startValue, endValue)
        {
        }

        public override float Interpolate(float coef)
        {
            float endCoef = (float)Math.Sin(coef * (Math.PI / 2.0D));
            float startCoef = 1.0F - endCoef;
            float result = startValue * startCoef + endCoef * endValue;
            return result;
        }
    }
}
