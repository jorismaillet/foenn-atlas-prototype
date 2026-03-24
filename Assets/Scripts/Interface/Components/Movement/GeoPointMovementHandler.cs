using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Components.Commons.Containers;
using Assets.Scripts.Components.Commons.Holders;
using Assets.Scripts.Components.Layers.OpenStreetMap;
using Assets.Scripts.Helpers;
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
            if (worldCamera.orthographicSize == cameraZoom && worldCamera.transform.position == cameraPosition)
                return;

            cameraZoom = worldCamera.orthographicSize;
            cameraPosition = worldCamera.transform.position;

            UpdatePositions();
        }

        private void UpdatePositions()
        {
            if (tileGridRenderer == null || worldCamera == null)
                return;

            foreach (Holder<GeoPoint> holder in pointHolders)
            {
                if (holder == null)
                    continue;

                Vector3 screenPos = GeoHelper.GeoToScreenPoint(tileGridRenderer, worldCamera, holder.element, zOffset);
                SetPosition(holder, screenPos);
            }
        }

        private void SetPosition(Holder<GeoPoint> holder, Vector3 screenPos)
        {
            GeoHelper.TrySetAnchoredPositionFromScreenPoint(holder.GetComponent<RectTransform>(), screenPos);
        }
    }
}
