using Assets.Scripts.Unity.Commons.Behaviours;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Unity.Scenes.Home.World.Views
{
    public abstract class OneFingerHandler : BaseBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private bool pressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            pressed = Input.touchCount == 1 || (Input.touchCount == 0 && eventData.button == PointerEventData.InputButton.Left);
            OnPointerUpdated(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressed = false;
            OnPointerUpdated(eventData);
        }

        private void OnPointerUpdated(PointerEventData eventData)
        {
            if (pressed)
            {
                OneFingerPressed(eventData.position);
            }
            else
            {
                OneFingerReleased(eventData.position);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (pressed)
            {
                OneFingerMoved(eventData.position);
            }
        }

        public abstract void OneFingerPressed(Vector3 screenPoint);
        public abstract void OneFingerReleased(Vector3 screenPoint);
        public abstract void OneFingerMoved(Vector3 screenPoint);
    }
}
