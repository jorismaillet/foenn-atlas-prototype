using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity.Commons.Behaviours {
    public abstract class DebouncedBehaviour : BaseBehaviour {
        private DateTime startedAt = DateTime.MinValue;
        public abstract double debounceTimeMillis { get; }

        public virtual void OnDebounceAttemp() {
            startedAt = DateTime.UtcNow;
        }

        public abstract void OnDebounceSuccess();
        public abstract void OnDebounceProgress(double elapsedTime);

        private void Update() {
            if (startedAt.Equals(DateTime.MinValue)) {
                return;
            }
            var elapsedTime = DateTime.UtcNow.Subtract(startedAt).TotalMilliseconds;
            OnDebounceProgress(elapsedTime);
            if (elapsedTime >= debounceTimeMillis) {
                startedAt = DateTime.MinValue;
                OnDebounceSuccess();
            }
        }
    }
}
