namespace Assets.Scripts.Unity.Common.Utils
{
    using Assets.Scripts.Unity.Commons.UIAnimations;
    using System.Collections.Generic;

    public class UIAnimator : UnityEngine.MonoBehaviour
    {
        private static List<UIAnimation> animations = new List<UIAnimation>();

        private static List<UIAnimationList> animationsLists = new List<UIAnimationList>();

        //TODO Dictionary with target object as a key, to automatically cancel previous animation
        public static UIAnimation Animate(UIAnimation newAnimation)
        {
            animations.Add(newAnimation);
            return newAnimation;
        }

        public static UIAnimationList Animate(UIAnimationList newAnimationsList)
        {
            animationsLists.Add(newAnimationsList);
            return newAnimationsList;
        }

        public static void CancelAnimation(UIAnimation animation)
        {
            animations.Remove(animation);
        }

        public static void CancelAnimation(UIAnimationList animationsList)
        {
            animationsLists.Remove(animationsList);
        }

        public static void CancelAllAnimations()
        {
            animations.Clear();
            animationsLists.Clear();
        }

        void FixedUpdate()
        {
            animations.RemoveAll(animation =>
            {
                animation.Animate(UnityEngine.Time.fixedDeltaTime);
                return animation.Ended();
            });
            animationsLists.RemoveAll(animationsList =>
            {
                animationsList.Animate(UnityEngine.Time.fixedDeltaTime);
                return animationsList.Ended();
            });
        }
    }
}
