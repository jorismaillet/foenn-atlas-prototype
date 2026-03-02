using Assets.Scripts.Foenn.Atlas.Layers;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap
{
    public class PointmapUiOverlay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] TileGridRenderer tileGridRenderer;
        [SerializeField] Camera worldCamera;
        [SerializeField] Canvas canvas;
        [SerializeField] RectTransform container;
        [SerializeField] GameObject geoPointPrefab;

        [Header("Appearance")]
        [SerializeField] PointmapUiSettings uiSettings = new PointmapUiSettings();

        [Header("Culling")]
        [SerializeField] bool cullOutsideGrid = true;
        [SerializeField] float cullMarginTiles = 1.5f;

        public event Action<GeoMeasure> PointClicked;

        readonly List<GeoMeasure> _measures = new();
        readonly List<RectTransform> _buttons = new();

        void Awake()
        {
            if (worldCamera == null)
                worldCamera = Camera.main;

            if (canvas == null)
                canvas = GetComponentInParent<Canvas>();

            if (container == null && canvas != null)
                container = canvas.transform as RectTransform;
        }

        public void SetMeasures(IReadOnlyList<GeoMeasure> measures)
        {
            _measures.Clear();
            if (measures != null) _measures.AddRange(measures);

            RebuildButtons();
        }

        public void Clear()
        {
            _measures.Clear();
            RebuildButtons();
        }

        void RebuildButtons()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i] != null)
                    Destroy(_buttons[i].gameObject);
            }
            _buttons.Clear();

            if (_measures.Count == 0) return;
            if (geoPointPrefab == null) throw new InvalidOperationException("PointmapUiOverlay: buttonPrefab is not assigned.");
            if (container == null) throw new InvalidOperationException("PointmapUiOverlay: canvas/container is not assigned.");

            uiSettings?.Validate();

            for (int i = 0; i < _measures.Count; i++)
            {
                var measure = _measures[i];
                var geoPointGO = Instantiate(geoPointPrefab, container);
                var rect = (RectTransform)geoPointGO.transform;

                rect.sizeDelta = Vector2.one * uiSettings.pointDiameterPx;

                var image = geoPointGO.GetComponent<Image>();

                _buttons.Add(rect);
            }
        }

        void LateUpdate()
        {
            if (_measures.Count == 0) return;
            if (tileGridRenderer == null || worldCamera == null || container == null) return;

            uiSettings?.Validate();

            int zoom = tileGridRenderer.mapZoom;
            int halfGridSize = tileGridRenderer.gridSize / 2;
            float tileWorldSize = tileGridRenderer.tileToWorldSize;
            double cullRadiusTiles = halfGridSize + (cullOutsideGrid ? cullMarginTiles : 1e9);

            GeoPoint mapCenter = tileGridRenderer.franceCenter;
            double centerTileX = SlippyMapMath.LonToTileX(mapCenter.lon, zoom);
            double centerTileY = SlippyMapMath.LatToTileY(mapCenter.lat, zoom);

            for (int i = 0; i < _measures.Count && i < _buttons.Count; i++)
            {
                var measure = _measures[i];
                var rect = _buttons[i];
                if (rect == null) continue;

                double tileX = SlippyMapMath.LonToTileX(measure.point.lon, zoom);
                double tileY = SlippyMapMath.LatToTileY(measure.point.lat, zoom);

                double dxTiles = tileX - centerTileX;
                double dyTiles = tileY - centerTileY;

                if (cullOutsideGrid && (Math.Abs(dxTiles) > cullRadiusTiles || Math.Abs(dyTiles) > cullRadiusTiles))
                {
                    rect.gameObject.SetActive(false);
                    continue;
                }

                rect.gameObject.SetActive(true);

                float localX = (float)(dxTiles * tileWorldSize);
                float localY = (float)(-dyTiles * tileWorldSize);

                // Put UI point slightly in front of the map plane.
                Vector3 worldPos = tileGridRenderer.transform.TransformPoint(new Vector3(localX, localY, -0.01f));
                Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

                if (screenPos.z <= 0f)
                {
                    rect.gameObject.SetActive(false);
                    continue;
                }

                // Screen Space Overlay: camera is null for the conversion.
                RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPos, null, out var localPos);
                rect.anchoredPosition = localPos;
            }
        }
    }
}
