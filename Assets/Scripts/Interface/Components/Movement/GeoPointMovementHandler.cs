using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Components.Commons.Containers;
using Assets.Scripts.Components.Commons.Holders;
using Assets.Scripts.Components.Layers.OpenStreetMap;
using Assets.Scripts.Models.Geo;
using UnityEngine;

namespace Assets.Scripts.Components.Movement
{
    public class GeoPointMovementHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] OpenStreetMapGridRenderer tileGridRenderer;

        [SerializeField] Camera worldCamera;

        [SerializeField] Canvas canvas;

        [SerializeField] PrefabsContainer container;

        [Header("Settings")]
        public float zOffset = -0.01f;

        private List<Holder<GeoPoint>> pointHolders = new List<Holder<GeoPoint>>();

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
            pointHolders.Clear();
            pointHolders.AddRange(container.elements.Select(go => go.GetComponent<Holder<GeoPoint>>()).ToList());
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

        private void UpdatePositions()
        {
            int mapZoom = tileGridRenderer.mapZoom;
            GeoPoint mapCenter = tileGridRenderer.franceCenter;
            float tileWorldSize = tileGridRenderer.tileToWorldSize;
            double centerTileX = TileGridHelper.LonToTileX(mapCenter.lon, mapZoom);
            double centerTileY = TileGridHelper.LatToTileY(mapCenter.lat, mapZoom);

            foreach (Holder<GeoPoint> holder in pointHolders)
            {
                var point = holder.element;
                var rect = holder.GetComponent<RectTransform>();
                if (rect == null) continue;

                var parentRect = rect.parent as RectTransform;
                if (parentRect == null) continue;

                double tileX = TileGridHelper.LonToTileX(point.lon, mapZoom);
                double tileY = TileGridHelper.LatToTileY(point.lat, mapZoom);

                double dxTiles = tileX - centerTileX;
                double dyTiles = tileY - centerTileY;

                float localX = (float)(dxTiles * tileWorldSize);
                float localY = (float)(-dyTiles * tileWorldSize);

                Vector3 worldPos = tileGridRenderer.transform.TransformPoint(new Vector3(localX, localY, zOffset));
                Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

                if (screenPos.z <= 0f)
                    continue;

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, null, out var localPos))
                    rect.anchoredPosition = localPos;
            }
        }
    }
}
