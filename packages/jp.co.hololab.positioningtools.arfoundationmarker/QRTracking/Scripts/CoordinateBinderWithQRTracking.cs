using System;
using System.Collections;
using System.Collections.Generic;
using HoloLab.ARFoundationQRTracking;
using HoloLab.PositioningTools.CoordinateSerialization;
using HoloLab.PositioningTools.CoordinateSystem;
using UnityEngine;

namespace HoloLab.PositioningTools.ARFoundationMarker
{
    public class CoordinateBinderWithQRTracking : MonoBehaviour
    {
        private CoordinateManager coordinateManager;
        private ARFoundationQRTracker qrTracker;

        private readonly Dictionary<ARTrackedQRImage, ICoordinateInfo> coordinateInfoDictionary = new Dictionary<ARTrackedQRImage, ICoordinateInfo>();
        private readonly CoordinateSerializer coordinateSerializer = new CoordinateSerializer();

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
                if (coordinateSerializer.TryDeserialize(addedQR.Text, out var coordinateInfo))
                {
                    coordinateInfoDictionary[addedQR] = coordinateInfo;
                }

                if (addedQR.TrackingReliable)
                {
                    BindSpaceCoordinates(addedQR);
                    BindWorldCoordinates(addedQR);
                }
            }

            foreach (var updatedQR in eventArgs.Updated)
            {
                if (updatedQR.TrackingReliable)
                {
                    BindSpaceCoordinates(updatedQR);
                    BindWorldCoordinates(updatedQR);
                }
            }

            foreach (var removedQR in eventArgs.Removed)
            {
                // Space binding
                var spaceBinding = ARTrackedQRImageToSpaceBinding(removedQR);
                coordinateManager.UnbindSpace(spaceBinding);

                // World binding
                coordinateInfoDictionary.Remove(removedQR);
            }
        }

        private void BindSpaceCoordinates(ARTrackedQRImage qr)
        {
            var spaceBinding = ARTrackedQRImageToSpaceBinding(qr);
            coordinateManager.BindSpace(spaceBinding);
        }

        private void BindWorldCoordinates(ARTrackedQRImage qr)
        {
            if (coordinateInfoDictionary.TryGetValue(qr, out var coordinateInfo) == false)
            {
                return;
            }

            if (TryConvertARTrackedQRImageToWorldBinding(qr, coordinateInfo, out var worldBinding))
            {
                coordinateManager.BindCoordinates(worldBinding);
            }
        }

        private static SpaceBinding ARTrackedQRImageToSpaceBinding(ARTrackedQRImage qr)
        {
            var qrTransform = qr.transform;
            var pose = new Pose(qrTransform.position, qrTransform.rotation);
            var spaceBinding = new SpaceBinding(pose, spaceType, qr.Text);
            return spaceBinding;
        }

        private static bool TryConvertARTrackedQRImageToWorldBinding(ARTrackedQRImage qr, ICoordinateInfo coordinateInfo, out WorldBinding worldBinding)
        {
            switch (coordinateInfo)
            {
                case GeodeticPositionWithHeading geodeticPositionWithHeading:
                    {
                        var qrTransform = qr.transform;
                        var position = qrTransform.position;

                        // TODO
                        // var rotation = Quaternion.identity;
                        var rotation = qrTransform.rotation;
                        var pose = new Pose(position, rotation);

                        var geodeticRotation = Quaternion.AngleAxis(geodeticPositionWithHeading.Heading, Vector3.up);
                        var geodeticPose = new GeodeticPose(geodeticPositionWithHeading.GeodeticPosition, geodeticRotation);
                        worldBinding = new WorldBinding(pose, geodeticPose);
                        return true;
                    }
            }

            worldBinding = null;
            return false;
        }
    }
}