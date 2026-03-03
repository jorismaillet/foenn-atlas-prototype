using Assets.Scripts.Foenn.Atlas.Layers;
using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Foenn.Atlas.Visualisations;
using Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap.RawImage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Foenn.Atlas.Visualisations.Heatmap
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class HeatmapWorldOverlay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] TileGridRenderer tileGridRenderer;

        [Header("Material")]
        [SerializeField] Material material;

        [Header("Heatmap")]
        [SerializeField] float idwPower = 2f;
        [Min(1)] [SerializeField] int maxNeighbors = 16;
        [Min(1f)] [SerializeField] float maxRadiusPx = 120f;
        [Range(0f, 1f)] [SerializeField] float alpha = 0.85f;
        [SerializeField] float tempMin = -10f;
        [SerializeField] float tempMax = 40f;
        [Min(1)] [SerializeField] int cellSizePx = 32;

        [Header("Mask (optional)")]
        [SerializeField] Texture2D mask;
        [SerializeField] BBox maskBBox = new BBox(-5.5f, 41.0f, 20.0f, 51.5f);
        [SerializeField] bool reprojectMaskToTileGrid = true;

        [Header("Depth")]
        [SerializeField] float zOffset = -0.01f;

        IReadOnlyList<GeoMeasure> _measures;

        MeshFilter _meshFilter;
        MeshRenderer _meshRenderer;
        Mesh _mesh;

        Material _runtimeMaterial;

        Texture2D _texture;

        int _lastZoom;
        int _lastGridSize;
        GeoPoint _lastCenter;
        float _lastTileWorldSize;

        void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();

            EnsureMesh();
            EnsureMaterial();
        }

        void OnEnable()
        {
            RebuildAll();
        }

        public void SetMeasures(IReadOnlyList<GeoMeasure> measures)
        {
            _measures = measures;
            RebuildTexture();
        }

        public void Clear()
        {
            _measures = null;
            RebuildTexture();
        }

        void LateUpdate()
        {
            if (tileGridRenderer == null) return;

            if (_lastZoom != tileGridRenderer.mapZoom ||
                _lastGridSize != tileGridRenderer.gridSize ||
                _lastTileWorldSize != tileGridRenderer.tileToWorldSize ||
                !_lastCenter.Equals(tileGridRenderer.franceCenter))
            {
                RebuildAll();
            }
        }

        void RebuildAll()
        {
            if (tileGridRenderer == null) return;

            _lastZoom = tileGridRenderer.mapZoom;
            _lastGridSize = tileGridRenderer.gridSize;
            _lastCenter = tileGridRenderer.franceCenter;
            _lastTileWorldSize = tileGridRenderer.tileToWorldSize;

            UpdateMeshToTileGrid();
            RebuildTexture();
        }

        void EnsureMesh()
        {
            if (_mesh != null) return;

            _mesh = new Mesh { name = "HeatmapOverlayQuad" };
            _meshFilter.sharedMesh = _mesh;
        }

        void EnsureMaterial()
        {
            if (_runtimeMaterial != null) return;

            Material source = material;
            if (source == null)
            {
                var shader = Shader.Find("Unlit/Transparent");
                if (shader != null)
                    source = new Material(shader);
            }

            if (source == null)
                return;

            // Instantiate to avoid modifying a shared asset material at runtime.
            _runtimeMaterial = new Material(source);
            _runtimeMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            // Best-effort: ensure we don't occlude the map with fully transparent pixels.
            if (_runtimeMaterial.HasProperty("_ZWrite"))
                _runtimeMaterial.SetInt("_ZWrite", 0);

            _meshRenderer.sharedMaterial = _runtimeMaterial;
        }

        void UpdateMeshToTileGrid()
        {
            if (tileGridRenderer == null || _mesh == null) return;

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

            // Mesh is built in the local space of the TileGridRenderer.
            // Recommended: make this GameObject a child of the TileGridRenderer transform.
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            var vertices = new Vector3[4]
            {
                new Vector3(left, bottom, zOffset),
                new Vector3(right, bottom, zOffset),
                new Vector3(right, top, zOffset),
                new Vector3(left, top, zOffset),
            };

            var uvs = new Vector2[4]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f),
                new Vector2(0f, 1f),
            };

            var triangles = new int[6] { 0, 2, 1, 0, 3, 2 };

            _mesh.Clear(false);
            _mesh.vertices = vertices;
            _mesh.uv = uvs;
            _mesh.triangles = triangles;
            _mesh.RecalculateBounds();
        }

        void RebuildTexture()
        {
            if (tileGridRenderer == null || _runtimeMaterial == null) return;

            var settings = new HeatmapSettings(idwPower, maxNeighbors, maxRadiusPx, cellSizePx);
            var rawImageSettings = new HeatmapRawImageSettings(alpha, tempMin, tempMax);

            var tex = HeatmapGenerator.BuildTileGridRawImageTexture(
                _measures ?? Array.Empty<GeoMeasure>(),
                settings,
                tileGridRenderer.franceCenter,
                tileGridRenderer.mapZoom,
                tileGridRenderer.gridSize,
                rawImageSettings,
                mapMask: mask,
                maskBBox: maskBBox,
                reprojectMaskToTileGrid: reprojectMaskToTileGrid
            );

            if (_texture != null)
                Destroy(_texture);

            _texture = tex;
            _runtimeMaterial.mainTexture = _texture;
        }

        void OnDestroy()
        {
            if (_texture != null)
                Destroy(_texture);

            if (_mesh != null)
                Destroy(_mesh);

            if (_runtimeMaterial != null)
                Destroy(_runtimeMaterial);
        }
    }
}
