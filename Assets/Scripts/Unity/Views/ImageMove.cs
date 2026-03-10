namespace Assets.Scripts.Unity.Scenes.Home.World.Views
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class ImageMove : OneFingerHandler
    {
        public Image image;

        private RectTransform imageTransform;

        private RectTransform containerTransform;

        public Vector3 initialPos;

        public Vector3 pressedPoint, dragPoint;

        private void Awake()
        {
            imageTransform = image.GetComponent<RectTransform>();
            containerTransform = transform.parent.GetComponent<RectTransform>();
        }

        public override void OneFingerPressed(Vector3 screenPoint)
        {
            pressedPoint = GamePoint(screenPoint);
            initialPos = LocalPosition(imageTransform);
        }

        public override void OneFingerMoved(Vector3 screenPoint)
        {
            dragPoint = GamePoint(screenPoint);
            Vector3 offsetPosition = initialPos + (dragPoint - pressedPoint);
        }

        private void SetPosition(Vector3 position)
        {
            imageTransform.localPosition = position;
        }

        private Vector3 FinalPosition(Vector3 position)
        {
            var limitFactor = 2;
            var zoneWidth = image.sprite.rect.width;
            var zoneHeight = image.sprite.rect.height;
            var zoomValue = imageTransform.localScale.x;
            return new Vector3(
                Math.Min(Math.Max(position.x, ((zoneWidth * zoomValue) / -limitFactor)), ((zoneWidth * zoomValue) / limitFactor)),
                Math.Min(Math.Max(position.y, ((zoneHeight * zoomValue) / -limitFactor)), ((zoneHeight * zoomValue) / limitFactor)),
                0
            );
        }

        public override void OneFingerReleased(Vector3 screenPoint)
        {
        }

        private Vector3 LocalPosition(RectTransform rectTransform)
        {
            return new Vector3(
                rectTransform.localPosition.x,
                rectTransform.localPosition.y
            );
        }

        private Vector3 GamePoint(Vector3 screenPoint)
        {
            return new Vector3(
                (screenPoint.x / Screen.width) * containerTransform.rect.width,
                (screenPoint.y / Screen.height) * containerTransform.rect.height
            );
        }
    }
}
