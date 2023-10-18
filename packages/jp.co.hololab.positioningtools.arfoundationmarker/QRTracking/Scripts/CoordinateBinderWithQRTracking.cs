using System;
using System.Collections;
using HoloLab.ARFoundationQRTracking;
using HoloLab.PositioningTools.CoordinateSystem;
using UnityEngine;

namespace HoloLab.PositioningTools.Immersal
{
    public class CoordinateBinderWithQRTracking : MonoBehaviour
    {
        private CoordinateManager coordinateManager;
        private ARFoundationQRTracker qrTracker;

        private const string spaceType = SpaceOrigin.SpaceTypeMarker;

        private void Start()
        {
            coordinateManager = CoordinateManager.Instance;

            qrTracker = FindObjectOfType<ARFoundationQRTracker>();
            qrTracker.OnTrackedQRImagesChanged += QRTracker_OnTrackedQRImagesChanged;
        }

        private void QRTracker_OnTrackedQRImagesChanged(ARTrackedQRImagesChangedEventArgs eventArgs)
        {
            foreach (var addedQR in eventArgs.Added)
            {
                Debug.Log($"Marker detected: {addedQR.Text}");
                // Instantiate(qrVisualizerPrefab, addedQR.transform);
            }

            foreach (var removedQR in eventArgs.Removed)
            {
                Debug.Log($"Marker lost: {removedQR.Text}");
                var spaceBinding = new SpaceBinding(removedQR.transform, spaceType, removedQR.Text);
                coordinateManager.UnbindSpace(spaceBinding);
            }
        }
    }
}