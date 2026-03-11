using UnityEngine;

namespace Assets.Scripts.Components.Commons.Utils
{
    //TODO: handle root canvas scale
    public class PanelUtil
    {
        public static float GetHeight(RectTransform rectTransform)
        {
            return Mathf.Round(rectTransform.rect.height);
        }

        public static void SetHeight(RectTransform rectTransform, float height)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }

        public static float GetWidth(RectTransform rectTransform)
        {
            return Mathf.Round(rectTransform.rect.width);
        }

        public static void SetWidth(RectTransform rectTransform, float width)
        {
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        }

        public static void SetRelativeWidth(RectTransform content, RectTransform container, float coef)
        {
            SetWidth(content, coef * GetWidth(container));
        }

        public static void SetRelativeHeight(RectTransform content, RectTransform container, float coef)
        {
            SetHeight(content, coef * GetHeight(container));
        }

        public static void SetX(RectTransform rectTransform, float x)
        {
            rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
        }

        public static void SetY(RectTransform rectTransform, float y)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
        }

        public static void AddY(RectTransform rectTransform, float y)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + y);
        }

        public static float GetX(RectTransform rectTransform)
        {
            return rectTransform.anchoredPosition.x;
        }

        public static float GetY(RectTransform rectTransform)
        {
            return rectTransform.anchoredPosition.y;
        }
    }
}
