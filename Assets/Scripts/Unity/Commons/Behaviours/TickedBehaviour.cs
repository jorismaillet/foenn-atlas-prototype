using System;

namespace Assets.Scripts.Unity.Commons.Behaviours {
    public abstract class TickedBehaviour : BaseBehaviour {
        private DateTime lastTick = DateTime.UtcNow;
        public abstract double tickTimeMillis { get; }

        private void Update() {
            if (DateTime.UtcNow.Subtract(lastTick).TotalMilliseconds <= tickTimeMillis) {
                return;
            }
            lastTick = DateTime.UtcNow;
            OnTick();
        }

        public abstract void OnTick();
    }
}
