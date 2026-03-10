namespace Assets.Scripts.Unity.Commons.UIAnimations.Interpolations
{
    using System;

    public class BounceInterpolation : Interpolation
    {
        public BounceInterpolation(Action<float> action, float startValue, float endValue) : base(action, startValue, endValue)
        {
        }

        public override float Interpolate(float coef)
        {
            float startCoef = (float)Math.Cos(coef * (Math.PI / 2.0D));
            float endCoef = 1.0F - startCoef;
            return startValue * startCoef + endCoef * endValue;
        }
    }
}
