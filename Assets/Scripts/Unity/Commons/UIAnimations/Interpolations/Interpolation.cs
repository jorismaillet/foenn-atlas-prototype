using System;

namespace Assets.Scripts.Unity.Commons.UIAnimations.Interpolations
{
    public abstract class Interpolation
    {
        public Action<float> action;
        public float startValue, endValue;

        protected Interpolation(Action<float> action, float startValue, float endValue)
        {
            this.action = action;
            this.startValue = startValue;
            this.endValue = endValue;
        }

        public abstract float Interpolate(float coef);

        public void Process(float coef)
        {
            action.Invoke(Interpolate(coef));
        }
    }
}