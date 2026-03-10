namespace Assets.Scripts.Unity.Commons.UIAnimations
{
    using Assets.Scripts.Common.Utils;
    using Assets.Scripts.Unity.Commons.UIAnimations.Interpolations;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class UIAnimation
    {
        private readonly List<Interpolation> interpolations;

        private GameObject from;

        private int durationMillis;

        private float elapsedTime = 0.0F;

        public Action callback;

        public UIAnimation(GameObject from, Interpolation interpolation, int durationMillis, Action callback = null)
        {
            this.from = from;
            this.interpolations = new List<Interpolation>() { interpolation };
            this.durationMillis = durationMillis;
            this.callback = callback;
        }

        public UIAnimation(GameObject from, List<Interpolation> interpolations, int durationMillis, Action callback = null)
        {
            this.from = from;
            this.interpolations = interpolations;
            this.durationMillis = durationMillis;
            this.callback = callback;
        }

        public void Animate(float deltaTime)
        {
            elapsedTime = Math.Min(elapsedTime + deltaTime * 1000.0F, durationMillis);
            float coef = MathUtil.Coef(0, durationMillis, elapsedTime);
            try
            {
                foreach (Interpolation interpolation in interpolations)
                {
                    interpolation.Process(coef);
                }
            }
            catch (MissingReferenceException e)
            {
                Debug.LogWarning(e, from);
                elapsedTime = durationMillis;
            }
            if (Ended() && callback != null)
            {
                callback();
            }
        }

        public bool Ended()
        {
            return elapsedTime >= durationMillis;
        }
    }
}
