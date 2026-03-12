using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Components.Movement
{
    public class CameraMover : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler
    {
        [Header("References")]
        [SerializeField] Camera targetCamera;

        [Header("Map plane")]
        [SerializeField] float mapPlaneZ = 0f;

        [Header("Pan")]
        [SerializeField] bool enablePan = true;

        [SerializeField] PointerEventData.InputButton mouseButton = PointerEventData.InputButton.Left;

        [Header("Zoom")]
        [SerializeField] bool enableZoom = true;

        [SerializeField] float zoomSpeed = 0.15f;

        [SerializeField] float minOrthoSize = 0.5f;

        [SerializeField] float maxOrthoSize = 50f;

        [SerializeField] float perspectiveDollySpeed = 5f;

        bool _dragging;

        Vector2 _lastScreenPos;

        int _activePointerId;

        void Awake()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!enablePan) return;
            if (eventData.button != mouseButton) return;
            if (targetCamera == null) return;

            _dragging = true;
            _activePointerId = eventData.pointerId;
            _lastScreenPos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId) return;
            _dragging = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!enablePan) return;
            if (!_dragging) return;
            if (eventData.pointerId != _activePointerId) return;
            if (targetCamera == null) return;

            if (!TryScreenToMapPlaneWorld(_lastScreenPos, out var prevWorld))
            {
                _lastScreenPos = eventData.position;
                return;
            }

            if (!TryScreenToMapPlaneWorld(eventData.position, out var curWorld))
            {
                _lastScreenPos = eventData.position;
                return;
            }

            Vector3 delta = prevWorld - curWorld;
            targetCamera.transform.position += new Vector3(delta.x, delta.y, 0f);
            _lastScreenPos = eventData.position;
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (!enableZoom) return;
            if (targetCamera == null) return;

            float scroll = eventData.scrollDelta.y;
            if (Mathf.Abs(scroll) < 0.001f) return;

            // Zoom around cursor: keep the map point under the cursor stable.
            if (!TryScreenToMapPlaneWorld(eventData.position, out var worldBefore))
                worldBefore = targetCamera.transform.position;

            if (targetCamera.orthographic)
            {
                float factor = Mathf.Exp(-scroll * zoomSpeed);
                float newSize = Mathf.Clamp(targetCamera.orthographicSize * factor, minOrthoSize, maxOrthoSize);
                targetCamera.orthographicSize = newSize;
            }
            else
            {
                // Dolly along camera forward.
                float dolly = -scroll * perspectiveDollySpeed;
                targetCamera.transform.position += targetCamera.transform.forward * dolly;
            }

            if (TryScreenToMapPlaneWorld(eventData.position, out var worldAfter))
            {
                Vector3 delta = worldBefore - worldAfter;
                targetCamera.transform.position += new Vector3(delta.x, delta.y, 0f);
            }
        }

        bool TryScreenToMapPlaneWorld(Vector2 screenPos, out Vector3 worldPos)
        {
            var ray = targetCamera.ScreenPointToRay(screenPos);
            var plane = new Plane(Vector3.forward, new Vector3(0f, 0f, mapPlaneZ));
            if (plane.Raycast(ray, out float enter))
            {
                worldPos = ray.GetPoint(enter);
                return true;
            }

            worldPos = default;
            return false;
        }
    }
}
