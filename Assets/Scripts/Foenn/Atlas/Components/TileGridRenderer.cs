using Assets.Scripts.Foenn.Atlas.Models.Geo;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Foenn.Atlas.Layers
{
    public class TileGridRenderer : MonoBehaviour
    {
        public GeoPoint franceCenter = new GeoPoint(46.50f, 2.00f);

        [Header("Slippy Tiles")]
        [Range(0, 19)] public int mapZoom = 6;
        [Range(1, 21)] public int gridSize = 7;
        public float tileToWorldSize = 1f;

        [Header("Tile Source")]
        public string urlTemplate = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";

        [Header("Rendering")]
        public Material tileMaterial;
        public bool generateOnStart = true;

        [Header("Performance")]
        [Range(1, 16)] public int maxConcurrentDownloads = 6;
        public bool verboseLogs = false;

        private int TileTexturePropertyId = ShaderPropertyIds.MainTex;

        private readonly Dictionary<string, Texture2D> _texCache = new();
        private readonly Dictionary<string, TileInstance> _tileInstances = new();
        private readonly HashSet<string> _downloadsInFlight = new();

        private int _activeDownloads;
        private int _pendingTileLoads;
        private bool _ownsRuntimeMaterial;

        private Mesh _sharedQuadMesh;
        private Material _sharedRuntimeMaterial;

        private static class ShaderPropertyIds
        {
            public static readonly int MainTex = Shader.PropertyToID("_MainTex");
        }

        private sealed class TileInstance
        {
            public GameObject go;
            public MeshRenderer renderer;
            public MaterialPropertyBlock props;
        }

        private void Start()
        {
            if (!generateOnStart)
                return;

            EnsureInitialized();
            StartCoroutine(BuildGrid());
        }

        [ContextMenu("Rebuild Grid")]
        public void Rebuild()
        {
            StopAllCoroutines();
            EnsureInitialized();
            ClearTiles();
            StartCoroutine(BuildGrid());
        }

        private void OnDestroy()
        {
            ClearTiles();

            if (_sharedQuadMesh != null)
            {
                Destroy(_sharedQuadMesh);
                _sharedQuadMesh = null;
            }

            if (_ownsRuntimeMaterial && _sharedRuntimeMaterial != null)
            {
                Destroy(_sharedRuntimeMaterial);
                _sharedRuntimeMaterial = null;
                _ownsRuntimeMaterial = false;
            }

            foreach (var kv in _texCache)
            {
                if (kv.Value != null)
                    Destroy(kv.Value);
            }
            _texCache.Clear();
        }

        private void EnsureInitialized()
        {
            if (gridSize % 2 == 0)
                gridSize += 1;

            if (tileMaterial == null)
            {
                Shader shader = Shader.Find("Unlit/Texture");
                tileMaterial = new Material(shader);
                _ownsRuntimeMaterial = true;
            }

            _sharedRuntimeMaterial = tileMaterial;

            if (_sharedQuadMesh == null)
                _sharedQuadMesh = CreateSharedQuadMesh();
        }

        private void ClearTiles()
        {
            foreach (var kv in _tileInstances)
            {
                if (kv.Value != null && kv.Value.go != null)
                    Destroy(kv.Value.go);
            }

            _tileInstances.Clear();
            _downloadsInFlight.Clear();
            _activeDownloads = 0;
            _pendingTileLoads = 0;
        }

        private IEnumerator BuildGrid()
        {
            double centerTileXf = TileGridHelper.LonToTileX(franceCenter.lon, mapZoom);
            double centerTileYf = TileGridHelper.LatToTileY(franceCenter.lat, mapZoom);

            int centerTileX = Mathf.FloorToInt((float)centerTileXf);
            int centerTileY = Mathf.FloorToInt((float)centerTileYf);

            int halfGridSize = gridSize / 2;

            float centerTileFractionX = (float)(centerTileXf - centerTileX);
            float centerTileFractionY = (float)(centerTileYf - centerTileY);
            Vector2 centerOffset = new Vector2(centerTileFractionX, centerTileFractionY);

            for (int tileOffsetY = -halfGridSize; tileOffsetY <= halfGridSize; tileOffsetY++)
            {
                int tileY = centerTileY + tileOffsetY;
                if (!TileGridHelper.IsValidTileY(tileY, mapZoom))
                    continue;

                for (int tileOffsetX = -halfGridSize; tileOffsetX <= halfGridSize; tileOffsetX++)
                {
                    int rawTileX = centerTileX + tileOffsetX;
                    int tileX = TileGridHelper.WrapTileX(rawTileX, mapZoom);

                    float localX = (tileOffsetX - centerOffset.x + 0.5f) * tileToWorldSize;
                    float localY = (-(tileOffsetY - centerOffset.y) - 0.5f) * tileToWorldSize;

                    string key = BuildCacheKey(mapZoom, tileX, tileY);

                    TileInstance tile = CreateTileInstance(localX, localY, tileToWorldSize);
                    tile.go.name = $"tile_{key}";
                    _tileInstances[key] = tile;

                    _pendingTileLoads++;
                    StartCoroutine(AssignTileTexture(tile, mapZoom, tileX, tileY));
                }
            }

            while (_pendingTileLoads > 0)
                yield return null;
        }

        private TileInstance CreateTileInstance(float localX, float localY, float size)
        {
            GameObject go = new GameObject("tile");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(localX, localY, 0f);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = new Vector3(size, size, 1f);

            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = _sharedQuadMesh;

            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = _sharedRuntimeMaterial;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            meshRenderer.allowOcclusionWhenDynamic = false;
            meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            return new TileInstance
            {
                go = go,
                renderer = meshRenderer,
                props = new MaterialPropertyBlock()
            };
        }

        private IEnumerator AssignTileTexture(TileInstance tile, int zoomLevel, int tileX, int tileY)
        {
            string cacheKey = BuildCacheKey(zoomLevel, tileX, tileY);

            try
            {
                if (tile == null || tile.go == null || tile.renderer == null)
                    yield break;

                if (_texCache.TryGetValue(cacheKey, out Texture2D cached) && cached != null)
                {
                    ApplyTexture(tile, cached);
                    yield break;
                }

                if (TryLoadTileFromDisk(zoomLevel, tileX, tileY, out Texture2D diskTex) && diskTex != null)
                {
                    _texCache[cacheKey] = diskTex;
                    ApplyTexture(tile, diskTex);
                    yield break;
                }

                if (_downloadsInFlight.Contains(cacheKey))
                {
                    while (_downloadsInFlight.Contains(cacheKey))
                        yield return null;

                    if (_texCache.TryGetValue(cacheKey, out cached) && cached != null)
                    {
                        ApplyTexture(tile, cached);
                        yield break;
                    }

                    if (TryLoadTileFromDisk(zoomLevel, tileX, tileY, out diskTex) && diskTex != null)
                    {
                        _texCache[cacheKey] = diskTex;
                        ApplyTexture(tile, diskTex);
                        yield break;
                    }

                    yield break;
                }

                while (_activeDownloads >= Mathf.Max(1, maxConcurrentDownloads))
                    yield return null;

                _activeDownloads++;
                _downloadsInFlight.Add(cacheKey);

                string url = urlTemplate
                    .Replace("{z}", zoomLevel.ToString())
                    .Replace("{x}", tileX.ToString())
                    .Replace("{y}", tileY.ToString());

                using UnityWebRequest req = UnityWebRequest.Get(url);
                req.timeout = 20;
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                {
                    if (verboseLogs)
                        Debug.LogWarning($"Tile download failed {cacheKey}: {req.error}");
                    yield break;
                }

                byte[] bytes = req.downloadHandler.data;
                if (bytes == null || bytes.Length == 0)
                    yield break;

                TryWriteTileToDisk(zoomLevel, tileX, tileY, bytes);

                Texture2D tex = DecodeTexture(bytes);
                if (tex == null)
                    yield break;

                _texCache[cacheKey] = tex;

                if (tile != null && tile.go != null && tile.renderer != null)
                    ApplyTexture(tile, tex);
            }
            finally
            {
                _downloadsInFlight.Remove(cacheKey);

                if (_activeDownloads > 0)
                    _activeDownloads--;

                if (_pendingTileLoads > 0)
                    _pendingTileLoads--;
            }
        }

        private void ApplyTexture(TileInstance tile, Texture2D tex)
        {
            if (tile == null || tile.renderer == null || tex == null)
                return;

            tile.props.Clear();
            tile.props.SetTexture(TileTexturePropertyId, tex);
            tile.renderer.SetPropertyBlock(tile.props);
        }

        private static string BuildCacheKey(int zoomLevel, int tileX, int tileY)
        {
            return $"{zoomLevel}/{tileX}/{tileY}";
        }

        private string GetTileCachePath(int zoomLevel, int tileX, int tileY)
        {
            return Path.Combine(
                Application.persistentDataPath,
                "TileCache",
                zoomLevel.ToString(),
                tileX.ToString(),
                $"{tileY}.png"
            );
        }

        private bool TryLoadTileFromDisk(int zoomLevel, int tileX, int tileY, out Texture2D tex)
        {
            string path = GetTileCachePath(zoomLevel, tileX, tileY);

            if (!File.Exists(path))
            {
                tex = null;
                return false;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                tex = DecodeTexture(bytes);
                return tex != null;
            }
            catch
            {
                tex = null;
                return false;
            }
        }

        private void TryWriteTileToDisk(int zoomLevel, int tileX, int tileY, byte[] bytes)
        {
            try
            {
                string path = GetTileCachePath(zoomLevel, tileX, tileY);
                string dir = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllBytes(path, bytes);
            }
            catch
            {
                // silence volontaire pour éviter de polluer/log spam
            }
        }

        private static Texture2D DecodeTexture(byte[] bytes)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);

            if (!tex.LoadImage(bytes, markNonReadable: true))
            {
                Destroy(tex);
                return null;
            }

            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            return tex;
        }

        private static Mesh CreateSharedQuadMesh()
        {
            Mesh mesh = new Mesh
            {
                name = "TileQuadShared"
            };

            mesh.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, 0f),
                new Vector3( 0.5f, -0.5f, 0f),
                new Vector3(-0.5f,  0.5f, 0f),
                new Vector3( 0.5f,  0.5f, 0f)
            };

            mesh.uv = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f)
            };

            mesh.triangles = new[]
            {
                0, 2, 1,
                2, 3, 1
            };

            mesh.RecalculateBounds();
            mesh.UploadMeshData(true);
            return mesh;
        }
    }
}