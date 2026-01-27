using UnityEngine;

namespace Assets.Scripts.Unity.Commons.Utils {
    public class VectorUtil {
        public static Vector2 NormalizedAngleOffset(Vector3 from, Vector3 to) {
            return new Vector2(to.x - from.x, to.z - from.z).normalized;
        }
    }
}
