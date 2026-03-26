using Assets.Scripts.Interface.Components.Layers;
using Assets.Scripts.Models.Geo;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class GeoHelper
    {
        public struct TileCenterData
        {
            public double centerTileXf;
            public double centerTileYf;
            public int centerTileX;
            public int centerTileY;
            public float fracX;
            public float fracY;
        }

        public static TileCenterData GetTileCenterData(GeoPoint mapCenter, int mapZoom)
        {
            var centerTileXf = TileGridHelper.LonToTileX(mapCenter.lon, mapZoom);
            var centerTileYf = TileGridHelper.LatToTileY(mapCenter.lat, mapZoom);
            var centerTileX = Mathf.FloorToInt((float)centerTileXf);
            var centerTileY = Mathf.FloorToInt((float)centerTileYf);

            return new TileCenterData
            {
                centerTileXf = centerTileXf,
                centerTileYf = centerTileYf,
                centerTileX = centerTileX,
                centerTileY = centerTileY,
                fracX = (float)(centerTileXf - centerTileX),
                fracY = (float)(centerTileYf - centerTileY)
            };
        }

        public static Vector3 GeoToScreenPoint(OpenStreetMapGridRenderer tileGridRenderer, Camera worldCamera, GeoPoint point, float zOffset = -0.01f)
        {
            var center = GetTileCenterData(tileGridRenderer.franceCenter, tileGridRenderer.mapZoom);
            Vector2 tileOffset = TileGridHelper.GeoToWorld(point, tileGridRenderer.mapZoom, center.centerTileXf, center.centerTileYf);

            float localX = tileOffset.x * tileGridRenderer.tileToWorldSize;
            float localY = tileOffset.y * tileGridRenderer.tileToWorldSize;

            Vector3 worldPos = tileGridRenderer.transform.TransformPoint(new Vector3(localX, localY, zOffset));
            return worldCamera.WorldToScreenPoint(worldPos);
        }

        public static bool TrySetAnchoredPositionFromScreenPoint(RectTransform rect, Vector3 screenPoint)
        {
            if (rect == null)
                return false;

            var parentRect = rect.parent as RectTransform;
            if (parentRect == null)
                return false;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, null, out var localPos))
                return false;

            rect.anchoredPosition = localPos;
            return true;
        }
    }
}
