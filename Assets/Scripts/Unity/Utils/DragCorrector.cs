using Assets.Scripts.Unity.Commons.Behaviours;
using Assets.Scripts.Unity.Config;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Unity.Common.Utils {
    public class DragCorrector : BaseBehaviour {
        private static readonly int preferredScreenTH = 6;
        private static readonly int preferredScreenPPI = 210;

        void Start() {
            int dragTH = preferredScreenTH * (int)Screen.dpi / preferredScreenPPI;

            EventSystem es = GetComponent<EventSystem>();

            if (es) es.pixelDragThreshold = dragTH;
        }
    }
}