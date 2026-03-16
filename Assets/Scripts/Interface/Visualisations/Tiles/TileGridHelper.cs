using System;
using Assets.Scripts.Models.Geo;
using UnityEngine;

namespace Assets.Scripts.Interface.Visualisations.Tiles
{
    public static class TileGridHelper
    {
        public const int TileSize = 256;

        const double Deg2Rad = Math.PI / 180.0;

        const double Inv360 = 1.0 / 360.0;

        const double InvPi = 1.0 / Math.PI;

        const double Half = 0.5;

        const double QuarterPi = Math.PI / 4.0;

        // Clamp used by WebMercator (avoid Inf near the poles)
        const double WebMercatorMaxLat = 85.05112878;

        /// <summary>
        /// Slippy-map tile X coordinate (WebMercator), continuous in [0, 2^z].
        /// </summary>
        public static double LonToTileX(double lon, int z)
        {
            // scale = 2^z
            double scale = (double)(1 << z);
            return (lon + 180.0) * (Inv360 * scale);
        }

        /// <summary>
        /// Inverse of <see cref="LonToTileX"/>.
        /// </summary>
        public static double TileXToLon(double tileX, int z)
        {
            double scale = (double)(1 << z);
            return tileX / scale * 360.0 - 180.0;
        }

        /// <summary>
        /// Slippy-map tile Y coordinate (WebMercator), continuous in [0, 2^z].
        /// </summary>
        public static double LatToTileY(double lat, int z)
        {
            // Prevent numeric issues at the poles.
            if (lat > WebMercatorMaxLat) lat = WebMercatorMaxLat;
            else if (lat < -WebMercatorMaxLat) lat = -WebMercatorMaxLat;

            double latRad = lat * Deg2Rad;
            double n = Math.Log(Math.Tan(QuarterPi + latRad * Half));
            double scale = (double)(1 << z);
            return (Half - (n * InvPi) * Half) * scale;
        }

        /// <summary>
        /// Inverse of <see cref="LatToTileY"/>.
        /// </summary>
        public static double TileYToLat(double tileY, int z)
        {
            double scale = (double)(1 << z);
            double n = Math.PI - 2.0 * Math.PI * tileY / scale;
            return Math.Atan(Math.Sinh(n)) / Deg2Rad;
        }

        public static Vector2 GeoToWorld(GeoPoint point, int z, double centerTileX, double centerTileY)
        {
            double tx = LonToTileX(point.lon, z);
            double ty = LatToTileY(point.lat, z);

            float x = (float)(tx - centerTileX);
            float y = (float)(-(ty - centerTileY));

            return new Vector2(x, y);
        }

        public static int WrapTileX(int x, int z)
        {
            int n = 1 << z;
            x %= n;
            if (x < 0) x += n;
            return x;
        }

        public static bool IsValidTileY(int y, int z)
        {
            int n = 1 << z;
            return y >= 0 && y < n;
        }
    }
}
