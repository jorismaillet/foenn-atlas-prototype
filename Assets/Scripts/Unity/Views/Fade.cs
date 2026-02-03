using Assets.Scripts.Unity.Commons.Behaviours;
using Assets.Scripts.Unity.Commons.UIAnimations;
using Assets.Scripts.Unity.Commons.UIAnimations.Interpolations;
using Assets.Scripts.Unity.Commons.Utils;
using UnityEngine;

namespace Assets.Scripts.Unity.Common.Views
{
    public class Fade : BaseBehaviour
    {
        public int fadeMillis;
        [Range(0, 1)]
        public float from, to;

        private void OnEnable()
        {
            ColorUtil.SetAlpha(GetComponent<CanvasGroup>(), from);
            Animate(new UIAnimation(gameObject, new LinearInterpolation(alpha => { ColorUtil.SetAlpha(GetComponent<CanvasGroup>(), alpha); }, from, to), fadeMillis));
        }
    }
}
