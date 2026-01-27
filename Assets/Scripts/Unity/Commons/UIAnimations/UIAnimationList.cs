using Assets.Scripts.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Unity.Commons.UIAnimations {
    public class UIAnimationList {
        private readonly List<UIAnimation> animations;
        public Action callback;

        public UIAnimationList(List<UIAnimation> animations, Action callback = null) {
            this.animations = animations;
            this.callback = callback;
        }

        public void Animate(float deltaTime) {
            animations[0].Animate(deltaTime);
            if (animations[0].Ended()) {
                animations.RemoveAt(0);
            }
            if (Ended() && callback != null) {
                callback();
            }
        }

        public bool Ended() {
            return animations.Empty();
        }
    }
}