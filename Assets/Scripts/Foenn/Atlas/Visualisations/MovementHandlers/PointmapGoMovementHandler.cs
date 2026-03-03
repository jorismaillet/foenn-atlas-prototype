using Assets.Scripts.Common.Extensions;
using Assets.Scripts.Foenn.Atlas.Layers;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Unity.Commons.Containers;
using Assets.Scripts.Unity.Commons.Holders;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap
{
    public class PointmapGoMovementHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] TileGridRenderer tileGridRenderer;
        [SerializeField] Camera worldCamera;
        [SerializeField] Canvas canvas;
        [SerializeField] PrefabsContainer container;

        [Header("Settings")]
        public float zOffset = -0.01f;


        private List<Holder<GeoMeasure>> measureHolders = new List<Holder<GeoMeasure>>();
        private void Awake()
        {
            container.elements.onChange.AddListener(elements =>
            {
                RefreshMeasureHolders();
                UpdatePositions();
            });
            RefreshMeasureHolders();
        }

        void RefreshMeasureHolders()
        {
            measureHolders.Replace(container.elements.Select(go => go.GetComponent<Holder<GeoMeasure>>()).ToList());
        }

        void OnEnable()
        {
            UpdatePositions();
        }

        private float cameraZoom;
        private Vector3 cameraPosition;

        void LateUpdate()
        {
            if (worldCamera.orthographicSize == this.cameraZoom && worldCamera.transform.position == this.cameraPosition)
            {
                return;
            }
            this.cameraZoom = worldCamera.orthographicSize;
            this.cameraPosition = worldCamera.transform.position;
            UpdatePositions();
        }
        private void UpdatePositions() {
            int mapZoom = tileGridRenderer.mapZoom;
            GeoPoint mapCenter = tileGridRenderer.franceCenter;
            float tileWorldSize = tileGridRenderer.tileToWorldSize;
            double centerTileX = SlippyMapMath.LonToTileX(mapCenter.lon, mapZoom);
            double centerTileY = SlippyMapMath.LatToTileY(mapCenter.lat, mapZoom);

            foreach (Holder<GeoMeasure> holder in measureHolders)
            {
                var measure = holder.element;
                var rect = holder.GetComponent<RectTransform>();
                if (rect == null) continue;

                var parentRect = rect.parent as RectTransform;
                if (parentRect == null) continue;

                double tileX = SlippyMapMath.LonToTileX(measure.point.lon, mapZoom);
                double tileY = SlippyMapMath.LatToTileY(measure.point.lat, mapZoom);

                double dxTiles = tileX - centerTileX;
                double dyTiles = tileY - centerTileY;

                float localX = (float)(dxTiles * tileWorldSize);
                float localY = (float)(-dyTiles * tileWorldSize);

                Vector3 worldPos = tileGridRenderer.transform.TransformPoint(new Vector3(localX, localY, zOffset));
                Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

                if (screenPos.z <= 0f)
                    continue;

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, worldCamera, out var localPos))
                    rect.anchoredPosition = localPos;
            }
        }
    }
}
