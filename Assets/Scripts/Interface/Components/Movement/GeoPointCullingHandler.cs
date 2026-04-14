using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interface.Components.Commons;
using Assets.Scripts.Interface.Components.Layers;
using Assets.Scripts.Models.Geo;
using UnityEngine;

namespace Assets.Scripts.Interface.Components.Movement
{
    public class GeoPointCullingHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] OpenStreetMapGridRenderer tileGridRenderer;

        [SerializeField] Camera worldCamera;

        [SerializeField] Canvas canvas;

        [SerializeField] PrefabsContainer container;

        [Header("Settings")]
        public float zOffset = -0.01f;

        [Min(1f)]
        [SerializeField] private float minPointDistancePx = 60f;

        private readonly List<Holder<GeoPoint>> allHolders = new List<Holder<GeoPoint>>();

        private readonly List<Holder<GeoPoint>> tooCloseToNeighbour = new List<Holder<GeoPoint>>();

        private readonly List<Holder<GeoPoint>> visible = new List<Holder<GeoPoint>>();

        private readonly List<Vector3> visibleScreenPositions = new List<Vector3>();

        private float cameraZoom;

        private void Awake()
        {
            container.elements.onChange.AddListener(elements => RefreshHolders());
            RefreshHolders();
        }

        private void RefreshHolders()
        {
            allHolders.Clear();
            allHolders.AddRange(container.elements
                .Select(go => go.GetComponent<Holder<GeoPoint>>())
                .Where(h => h != null));

            tooCloseToNeighbour.Clear();
            visible.Clear();
            visibleScreenPositions.Clear();

            FullReclassify();
        }

        private void LateUpdate()
        {
            if (worldCamera == null || tileGridRenderer == null)
                return;

            if (worldCamera.orthographicSize == cameraZoom)
                return;
            bool isZoom = worldCamera.orthographicSize < cameraZoom;
            cameraZoom = worldCamera.orthographicSize;
            PartialReclassify(isZoom);
        }

        private void FullReclassify()
        {
            float minDistSqr = minPointDistancePx * minPointDistancePx;

            foreach (var holder in visible)
                holder.gameObject.SetActive(false);

            tooCloseToNeighbour.Clear();
            visible.Clear();
            visibleScreenPositions.Clear();

            foreach (var holder in allHolders)
            {
                var screenPos = ScreenPos(holder);

                if (IsTooCloseToAny(screenPos, visibleScreenPositions, minDistSqr))
                {
                    tooCloseToNeighbour.Add(holder);
                    holder.gameObject.SetActive(false);
                    continue;
                }

                visible.Add(holder);
                visibleScreenPositions.Add(screenPos);
                GeoHelper.TrySetAnchoredPositionFromScreenPoint(holder.GetComponent<RectTransform>(), screenPos);
                holder.gameObject.SetActive(true);
            }
        }

        private void PartialReclassify(bool isZoom)
        {
            float minDistSqr = minPointDistancePx * minPointDistancePx;

            if (isZoom)
            {
                RebuildVisibleScreenPositions();

                for (int i = tooCloseToNeighbour.Count - 1; i >= 0; i--)
                {
                    var holder = tooCloseToNeighbour[i];
                    var screenPos = ScreenPos(holder);

                    if (IsTooCloseToAny(screenPos, visibleScreenPositions, minDistSqr))
                        continue;

                    tooCloseToNeighbour.RemoveAt(i);
                    visible.Add(holder);
                    visibleScreenPositions.Add(screenPos);
                    GeoHelper.TrySetAnchoredPositionFromScreenPoint(holder.GetComponent<RectTransform>(), screenPos);
                    holder.gameObject.SetActive(true);
                }
            }
            else
            {
                RebuildVisibleScreenPositions();
                var keptScreenPositions = new List<Vector3>();
                var toHide = new List<Holder<GeoPoint>>();

                for (int i = 0; i < visible.Count; i++)
                {
                    var screenPos = visibleScreenPositions[i];

                    if (IsTooCloseToAny(screenPos, keptScreenPositions, minDistSqr))
                        toHide.Add(visible[i]);
                    else
                        keptScreenPositions.Add(screenPos);
                }

                foreach (var holder in toHide)
                {
                    visible.Remove(holder);
                    tooCloseToNeighbour.Add(holder);
                    holder.gameObject.SetActive(false);
                }

                visibleScreenPositions.Clear();
                visibleScreenPositions.AddRange(keptScreenPositions);
            }
        }

        private void RebuildVisibleScreenPositions()
        {
            visibleScreenPositions.Clear();
            foreach (var holder in visible)
                visibleScreenPositions.Add(ScreenPos(holder));
        }

        private Vector3 ScreenPos(Holder<GeoPoint> holder)
        {
            return GeoHelper.GeoToScreenPoint(tileGridRenderer, worldCamera, holder.element, zOffset);
        }

        private static bool IsTooCloseToAny(Vector3 screenPos, List<Vector3> otherScreenPositions, float minDistanceSqr)
        {
            foreach (var other in otherScreenPositions)
            {
                if ((other - screenPos).sqrMagnitude < minDistanceSqr)
                    return true;
            }

            return false;
        }
    }
}
