using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine;

namespace Assets.Scripts.Unity.Common.Utils {
    [ExecuteInEditMode]
    public class RootCanvas : BaseBehaviour {
        public static Canvas instance;

        private void Awake() {
            instance = GetComponent<Canvas>();
        }
    }
}