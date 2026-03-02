using Assets.Scripts.Foenn.Atlas.Layers;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Pointmap
{
    internal static class PointmapWorldRenderer
    {
        /// <summary>
        /// Renders point markers in local/world space on top of a slippy-map tile grid.
        /// Each point is placed using WebMercator tile coordinates at the given zoom.
        ///
        /// If <paramref name="parent"/> is provided, markers are positioned in the local-space of that parent
        /// (recommended: pass your MapRoot / `TileGridRenderer.transform`).
        /// </summary>
        internal static List<GameObject> RenderPointMarkersOnTileGrid(
            IReadOnlyList<GeoMeasure> geoMeasures,
            GeoPoint mapCenter,
            int zoom,
            int gridSize,
            float tileWorldSize,
            PointmapSettings settings,
            Transform parent,
            float zOffset = -0.01f
        )
        {
            if (geoMeasures == null) throw new ArgumentNullException(nameof(geoMeasures));
            settings.Validate();

            double centerTileX = SlippyMapMath.LonToTileX(mapCenter.lon, zoom);
            double centerTileY = SlippyMapMath.LatToTileY(mapCenter.lat, zoom);

            int halfGridSize = gridSize / 2;
            double cullRadiusTiles = halfGridSize + 1.5;

            Color markerColor = settings.pointColor;
            markerColor.a *= settings.alpha;

            var markerMaterial = CreateMarkerMaterial(markerColor);
            float markerWorldDiameter = Mathf.Max(0.001f, (settings.pointRadiusPx * 2f / SlippyMapMath.TileSize) * tileWorldSize);

            var markers = new List<GameObject>(geoMeasures.Count);

            for (int i = 0; i < geoMeasures.Count; i++)
            {
                var m = geoMeasures[i];
                if (float.IsNaN(m.value)) continue;

                double tileX = SlippyMapMath.LonToTileX(m.point.lon, zoom);
                double tileY = SlippyMapMath.LatToTileY(m.point.lat, zoom);

                double dxTiles = tileX - centerTileX;
                double dyTiles = tileY - centerTileY;

                // Skip points outside of the currently rendered grid.
                if (Math.Abs(dxTiles) > cullRadiusTiles || Math.Abs(dyTiles) > cullRadiusTiles)
                    continue;

                float localX = (float)(dxTiles * tileWorldSize);
                float localY = (float)(-dyTiles * tileWorldSize);

                var markerGo = CreateMarkerQuad(localX, localY, zOffset, markerWorldDiameter, markerMaterial);
                if (parent != null)
                {
                    markerGo.transform.SetParent(parent, worldPositionStays: false);
                    markerGo.transform.localPosition = new Vector3(localX, localY, zOffset);
                }

                markers.Add(markerGo);
            }

            return markers;
        }

        static GameObject CreateMarkerQuad(float x, float y, float z, float size, Material sharedMaterial)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var col = go.GetComponent<Collider>();
            if (col) UnityEngine.Object.Destroy(col);

            go.transform.position = new Vector3(x, y, z);
            go.transform.localScale = new Vector3(size, size, 1f);

            var mr = go.GetComponent<MeshRenderer>();
            mr.sharedMaterial = sharedMaterial;
            return go;
        }

        static Material CreateMarkerMaterial(Color color)
        {
            // Prefer a pure color shader; fall back to Unlit/Texture + white texture.
            var shader = Shader.Find("Unlit/Color") ?? Shader.Find("Unlit/Texture");
            var mat = new Material(shader);

            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
            mat.color = color;

            if (mat.HasProperty("_MainTex"))
                mat.SetTexture("_MainTex", Texture2D.whiteTexture);

            return mat;
        }
    }
}
