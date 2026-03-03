using Assets.Scripts.Foenn.Atlas.Layers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Render
{
    public class PointmapRawImageMovementHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] TileGridRenderer tileGridRenderer;
        [SerializeField] Camera worldCamera;
        [SerializeField] RectTransform container;
        [SerializeField] RawImage rawImage;

        [Header("Depth")]
        [SerializeField] float localZ = -0.01f;

        RectTransform _rawImageRect;

        int _lastZoom;
        int _lastGridSize;
        float _lastTileWorldSize;
        Vector3 _lastCamPos;
        Quaternion _lastCamRot;
        float _lastCamOrthoSize;
        float _lastCamFov;
        Vector2 _lastContainerRectSize;

        bool _hasCache;

        void Awake()
        {
            if (rawImage == null)
                rawImage = GetComponent<RawImage>();

            _rawImageRect = (RectTransform)rawImage.transform;

            if (worldCamera == null)
                worldCamera = Camera.main;

            // IMPORTANT: anchoredPosition is relative to the RawImage parent.
            if (container == null && _rawImageRect != null)
                container = _rawImageRect.parent as RectTransform;

            if (_rawImageRect != null)
            {
                _rawImageRect.pivot = new Vector2(0.5f, 0.5f);
                _rawImageRect.anchorMin = new Vector2(0.5f, 0.5f);
                _rawImageRect.anchorMax = new Vector2(0.5f, 0.5f);
            }
        }

        void LateUpdate()
        {
            if (tileGridRenderer == null || worldCamera == null || container == null || rawImage == null)
                return;

            if (!ShouldUpdate())
                return;

            int zoom = tileGridRenderer.mapZoom;
            int gridSize = tileGridRenderer.gridSize;
            int halfGridSize = gridSize / 2;
            float tileWorldSize = tileGridRenderer.tileToWorldSize;

            double centerTileXf = SlippyMapMath.LonToTileX(tileGridRenderer.franceCenter.lon, zoom);
            double centerTileYf = SlippyMapMath.LatToTileY(tileGridRenderer.franceCenter.lat, zoom);
            int centerTileX = (int)Math.Floor(centerTileXf);
            int centerTileY = (int)Math.Floor(centerTileYf);
            float fracX = (float)(centerTileXf - centerTileX);
            float fracY = (float)(centerTileYf - centerTileY);

            float left = (-halfGridSize - fracX) * tileWorldSize;
            float right = (halfGridSize + 1 - fracX) * tileWorldSize;
            float top = (halfGridSize + fracY) * tileWorldSize;
            float bottom = (-halfGridSize - 1 + fracY) * tileWorldSize;

            Vector2 minLocal = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 maxLocal = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            var mapRoot = tileGridRenderer.transform;

            // Compute 4 corners without per-frame allocations.
            if (!TryUpdateMinMaxLocal(mapRoot.TransformPoint(new Vector3(left, top, localZ)), ref minLocal, ref maxLocal) ||
                !TryUpdateMinMaxLocal(mapRoot.TransformPoint(new Vector3(right, top, localZ)), ref minLocal, ref maxLocal) ||
                !TryUpdateMinMaxLocal(mapRoot.TransformPoint(new Vector3(right, bottom, localZ)), ref minLocal, ref maxLocal) ||
                !TryUpdateMinMaxLocal(mapRoot.TransformPoint(new Vector3(left, bottom, localZ)), ref minLocal, ref maxLocal))
            {
                rawImage.gameObject.SetActive(false);
                return;
            }

            rawImage.gameObject.SetActive(true);

            rawImage.gameObject.SetActive(true);

            Vector2 size = maxLocal - minLocal;
            _rawImageRect.sizeDelta = size;
            _rawImageRect.anchoredPosition = (minLocal + maxLocal) * 0.5f;
        }

        bool ShouldUpdate()
        {
            int zoom = tileGridRenderer.mapZoom;
            int gridSize = tileGridRenderer.gridSize;
            float tileWorldSize = tileGridRenderer.tileToWorldSize;

            Vector3 camPos = worldCamera.transform.position;
            Quaternion camRot = worldCamera.transform.rotation;
            float camOrthoSize = worldCamera.orthographic ? worldCamera.orthographicSize : 0f;
            float camFov = worldCamera.orthographic ? 0f : worldCamera.fieldOfView;

            Vector2 containerSize = container.rect.size;

            if (_hasCache &&
                zoom == _lastZoom &&
                gridSize == _lastGridSize &&
                Mathf.Abs(tileWorldSize - _lastTileWorldSize) < 1e-6f &&
                camPos == _lastCamPos &&
                camRot == _lastCamRot &&
                Mathf.Abs(camOrthoSize - _lastCamOrthoSize) < 1e-6f &&
                Mathf.Abs(camFov - _lastCamFov) < 1e-6f &&
                containerSize == _lastContainerRectSize)
            {
                return false;
            }

            _hasCache = true;
            _lastZoom = zoom;
            _lastGridSize = gridSize;
            _lastTileWorldSize = tileWorldSize;
            _lastCamPos = camPos;
            _lastCamRot = camRot;
            _lastCamOrthoSize = camOrthoSize;
            _lastCamFov = camFov;
            _lastContainerRectSize = containerSize;
            return true;
        }

        bool TryUpdateMinMaxLocal(Vector3 worldCorner, ref Vector2 minLocal, ref Vector2 maxLocal)
        {
            Vector3 screen = worldCamera.WorldToScreenPoint(worldCorner);
            if (screen.z <= 0f)
                return false;

            // Screen Space Overlay: camera is null for the conversion.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screen, null, out var local);
            minLocal = Vector2.Min(minLocal, local);
            maxLocal = Vector2.Max(maxLocal, local);
            return true;
        }
    }
}
