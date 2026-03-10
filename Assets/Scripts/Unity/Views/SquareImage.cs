namespace Assets.Scripts.Unity.Common.Views
{
    using Assets.Scripts.Unity.Commons.Behaviours;
    using Assets.Scripts.Unity.Commons.Utils;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class SquareImage : BaseBehaviour
    {
        public Image children;

        private void OnEnable()
        {
            Change();
        }

        private void OnRectTransformDimensionsChange()
        {
            Change();
        }

        private void Change()
        {
            Rect sourceRect = GetComponent<RectTransform>().rect;
            RectTransform destinationRectTransform = children.GetComponent<RectTransform>();
            float size = Math.Max(sourceRect.width, sourceRect.height);
            PanelUtil.SetWidth(destinationRectTransform, size);
            PanelUtil.SetHeight(destinationRectTransform, size);
        }
    }
}
