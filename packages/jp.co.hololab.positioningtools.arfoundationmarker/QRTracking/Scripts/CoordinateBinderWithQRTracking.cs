using System;
using System.Collections;
using HoloLab.ARFoundationQRTracking;
using HoloLab.PositioningTools.CoordinateSystem;
using UnityEngine;

namespace HoloLab.PositioningTools.ARFoundationMarker
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
            if (qrTracker == null)
            {
                Debug.LogError($"{nameof(ARFoundationQRTracker)} not found in scene. Please add {nameof(ARFoundationQRTracker)} to AR Session Origin");
            }
            else
            {
                qrTracker.OnTrackedQRImagesChanged += QRTracker_OnTrackedQRImagesChanged;
            }
        }

        private void QRTracker_OnTrackedQRImagesChanged(ARTrackedQRImagesChangedEventArgs eventArgs)
        {
            foreach (var addedQR in eventArgs.Added)
            {
                var spaceBinding = ARTrackedQRImageToSpaceBinding(addedQR);
                if (addedQR.TrackingReliable)
                {
                    coordinateManager.BindSpace(spaceBinding);
                }
            }

            foreach (var updatedQR in eventArgs.Updated)
            {
                var spaceBinding = ARTrackedQRImageToSpaceBinding(updatedQR);
                if (updatedQR.TrackingReliable)
                {
                    coordinateManager.BindSpace(spaceBinding);
                }
            }

            foreach (var removedQR in eventArgs.Removed)
            {
                var spaceBinding = ARTrackedQRImageToSpaceBinding(removedQR);
                coordinateManager.UnbindSpace(spaceBinding);
            }
        }

        private static SpaceBinding ARTrackedQRImageToSpaceBinding(ARTrackedQRImage qr)
        {
            var qrTransform = qr.transform;
            var pose = new Pose(qrTransform.position, qrTransform.rotation);
            var spaceBinding = new SpaceBinding(pose, spaceType, qr.Text);
            return spaceBinding;
        }
    }
}