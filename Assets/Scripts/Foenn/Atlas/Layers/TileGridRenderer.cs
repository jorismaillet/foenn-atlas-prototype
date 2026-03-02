using Assets.Scripts.Foenn.Atlas.Models.Geo;
using Assets.Scripts.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Foenn.Atlas.Layers
{

    public class TileGridRenderer : MonoBehaviour
    {
        public GeoPoint franceCenter = new GeoPoint(46.5f, 2.0f);

        [Header("Slippy Tiles")]
        [Range(0, 19)] public int mapZoom = 6;
        [Range(1, 21)] public int gridSize = 7; // must be odd for symmetric center (recommended)
        public float tileToWorldSize = 1f;        // 1 tile = 1 Unity unit

        [Header("Tile Source")]
        public string urlTemplate = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";

        [Header("Rendering")]
        public Material tileMaterial; // Unlit/Texture recommended
        public bool generateOnStart = true;

        // Simple in-memory cache (POC)
        private readonly Dictionary<string, Texture2D> _texCache = new();
        private readonly Dictionary<string, GameObject> _tileGos = new();
        private readonly HashSet<string> _downloadsInFlight = new();

        private string GetTileCachePath(int zoomLevel, int tileX, int tileY)
        {
            // persistentDataPath is the correct place for runtime cache on all platforms.
            return Path.Combine(Application.persistentDataPath, "TileCache", zoomLevel.ToString(), tileX.ToString(), $"{tileY}.png");
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

                // LoadImage will decode PNG/JPG bytes.
                tex = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
                if (!tex.LoadImage(bytes, markNonReadable: true))
                {
                    Destroy(tex);
                    tex = null;
                    return false;
                }

                tex.wrapMode = TextureWrapMode.Clamp;
                tex.filterMode = FilterMode.Bilinear;
                return true;
            }
            catch
            {
                tex = null;
                return false;
            }
        }

        private void Start()
        {
            if (!generateOnStart) return;
            MainThreadLog.Log($"Start");

            if (gridSize % 2 == 0) gridSize += 1; // force odd
            if (tileMaterial == null)
            {
                MainThreadLog.Log("TileGridRenderer: assign a Material (Unlit/Texture recommended). Creating a default Unlit material.");
                var shader = Shader.Find("Unlit/Texture");
                tileMaterial = new Material(shader);
            }

            StartCoroutine(BuildGrid());
        }

        [ContextMenu("Rebuild Grid")]
        public void Rebuild()
        {
            StopAllCoroutines();
            ClearTiles();
            StartCoroutine(BuildGrid());
        }

        private void ClearTiles()
        {
            foreach (var kv in _tileGos)
            {
                if (kv.Value != null) Destroy(kv.Value);
            }
            _tileGos.Clear();
            // Keep _texCache for POC speed; clear if you want fresh downloads
        }

        private IEnumerator BuildGrid()
        {
            MainThreadLog.Log($"BuildGrid");
            // Center tile coords (fractional)

            // Get center tile coordinates in the slippy-map tile space.
            // IMPORTANT: `GeoToWorld` returns a world-space delta with Y inverted; it is not suitable for tile indices.
            double centerTileXf = SlippyMapMath.LonToTileX(franceCenter.lon, mapZoom);
            double centerTileYf = SlippyMapMath.LatToTileY(franceCenter.lat, mapZoom);

            int centerTileX = (int)Mathf.Floor((float)centerTileXf);
            int centerTileY = (int)Mathf.Floor((float)centerTileYf);

            int halfGridSize = gridSize / 2;

            // Optional: offset grid so center tile aligns at (0,0)
            // You can also offset by fractional part to keep true center at the given lon/lat.
            float centerTileFractionX = (float)(centerTileXf - centerTileX);
            float centerTileFractionY = (float)(centerTileYf - centerTileY);

            // We'll place the *center lon/lat* at world (0,0).
            // That means the center tile (cX,cY) is shifted by its fractional part.
            Vector2 centerOffset = new Vector2(centerTileFractionX, centerTileFractionY);

            for (int tileOffsetY = -halfGridSize; tileOffsetY <= halfGridSize; tileOffsetY++)
            {
                int tileY = centerTileY + tileOffsetY;
                if (!SlippyMapMath.IsValidTileY(tileY, mapZoom)) continue;

                for (int tileOffsetX = -halfGridSize; tileOffsetX <= halfGridSize; tileOffsetX++)
                {
                    int rawTileX = centerTileX + tileOffsetX;
                    int tileX = SlippyMapMath.WrapTileX(rawTileX, mapZoom);

                    // Local position under this GameObject (tile center):
                    // - slippy tile coords are defined at the tile top-left corner
                    // - Unity quads are centered (pivot 0.5/0.5)
                    // => shift by +0.5 tile in X, and -0.5 tile in Y (because tileY grows downward).
                    float localX = (tileOffsetX - centerOffset.x + 0.5f) * tileToWorldSize;
                    float localY = (-(tileOffsetY - centerOffset.y) - 0.5f) * tileToWorldSize;

                    string key = $"{mapZoom}/{tileX}/{tileY}";
                    if (!_tileGos.ContainsKey(key))
                    {
                        var tileGo = CreateTileGO(localX, localY, tileToWorldSize);
                        tileGo.name = $"tile_{key}";
                        _tileGos[key] = tileGo;

                        // Start download/apply
                        yield return StartCoroutine(AssignTileTexture(tileGo, mapZoom, tileX, tileY));
                    }
                }
            }
            MainThreadLog.Log($"Done");
        }

        private GameObject CreateTileGO(float localX, float localY, float size)
        {
            // Quad is 1x1 by default in Unity (actually 1 unit). We scale it.
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            // Remove collider (not needed)
            var col = go.GetComponent<Collider>();
            if (col) Destroy(col);

            go.transform.SetParent(transform, worldPositionStays: false);
            go.transform.localPosition = new Vector3(localX, localY, 0f);
            go.transform.localScale = new Vector3(size, size, 1f);

            var mr = go.GetComponent<MeshRenderer>();
            mr.sharedMaterial = new Material(tileMaterial); // per-tile material instance (POC simple)

            return go;
        }

        private IEnumerator AssignTileTexture(GameObject tileGo, int zoomLevel, int tileX, int tileY)
        {
            string cacheKey = $"{zoomLevel}/{tileX}/{tileY}";
            var meshRenderer = tileGo.GetComponent<MeshRenderer>();

            if (_texCache.TryGetValue(cacheKey, out var cached) && cached != null)
            {
                MainThreadLog.Log($"Tile cache RAM hit: {cacheKey}");
                meshRenderer.sharedMaterial.mainTexture = cached;
                yield break;
            }

            // Persistent on-disk cache: never re-download the same tile.
            if (TryLoadTileFromDisk(zoomLevel, tileX, tileY, out var diskTex) && diskTex != null)
            {
                MainThreadLog.Log($"Tile cache DISK hit: {cacheKey}");
                _texCache[cacheKey] = diskTex;
                meshRenderer.sharedMaterial.mainTexture = diskTex;
                yield break;
            }

            // Avoid double network calls if multiple tiles request the same texture concurrently.
            if (_downloadsInFlight.Contains(cacheKey))
            {
                MainThreadLog.Log($"Tile download already in-flight, waiting: {cacheKey}");
                while (_downloadsInFlight.Contains(cacheKey))
                    yield return null;

                if (_texCache.TryGetValue(cacheKey, out cached) && cached != null)
                {
                    meshRenderer.sharedMaterial.mainTexture = cached;
                    yield break;
                }

                if (TryLoadTileFromDisk(zoomLevel, tileX, tileY, out diskTex) && diskTex != null)
                {
                    _texCache[cacheKey] = diskTex;
                    meshRenderer.sharedMaterial.mainTexture = diskTex;
                    yield break;
                }
            }

            _downloadsInFlight.Add(cacheKey);

            MainThreadLog.Log($"Tile download start: {cacheKey}");

            string url = urlTemplate
                .Replace("{z}", zoomLevel.ToString())
                .Replace("{x}", tileX.ToString())
                .Replace("{y}", tileY.ToString());

            using var req = UnityWebRequest.Get(url);
            req.timeout = 20;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                MainThreadLog.Log($"Tile download failed {cacheKey}: {req.error}");
                _downloadsInFlight.Remove(cacheKey);
                yield break;
            }

            MainThreadLog.Log($"Tile download success: {cacheKey}");

            byte[] bytes = req.downloadHandler.data;

            string path = GetTileCachePath(zoomLevel, tileX, tileY);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllBytes(path, bytes);

                MainThreadLog.Log($"Tile cached to disk: {cacheKey} -> {path}");
            }
            catch (System.Exception e)
            {
                MainThreadLog.Log($"Tile cache write failed {cacheKey}: {e.Message}");
            }

            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
            if (!tex.LoadImage(bytes, markNonReadable: true))
            {
                MainThreadLog.Log($"Tile decode failed {cacheKey}");
                Destroy(tex);
                _downloadsInFlight.Remove(cacheKey);
                yield break;
            }

            MainThreadLog.Log($"Tile decode success: {cacheKey}");

            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            _texCache[cacheKey] = tex;
            meshRenderer.sharedMaterial.mainTexture = tex;

            _downloadsInFlight.Remove(cacheKey);
        }
    }
}
